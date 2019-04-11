using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using CommonServiceLocator;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using WaitingClients.Models;
using WaitingClients.Models.Events;
using WaitingClients.Processors;

namespace WaitingClients.Gui.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        public ObservableCollection<Client> WaitingClients { get; set; } = new ObservableCollection<Client>();
        public ObservableCollection<Client> Q1 { get; set; } = new ObservableCollection<Client>();
        public ObservableCollection<Client> Q2 { get; set; } = new ObservableCollection<Client>();
        public ObservableCollection<Client> Q3 { get; set; } = new ObservableCollection<Client>();
        public ObservableCollection<Client> Q4 { get; set; } = new ObservableCollection<Client>();

        public string Log
        {
            get => _log;
            set => SetProperty(ref _log, value);
        }

        public DelegateCommand StartCommand { get; set; }
        public DelegateCommand StopCommand { get; set; }
        public DelegateCommand GenerateCommand { get; set; }

        public string GenerateClientsNr { get; set; }

        private readonly ConcurrentDictionary<Guid, ObservableCollection<Client>> _mapper = new ConcurrentDictionary<Guid, ObservableCollection<Client>>();
        private Store _store;
        private string _log;

        public MainWindowViewModel()
        {
            _eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

            StartCommand = new DelegateCommand(OnStart);
            StopCommand = new DelegateCommand(OnStop);
            GenerateCommand = new DelegateCommand(() =>
            {
                if (GenerateClientsNr != null)
                {
                    _eventAggregator.GetEvent<GenerateClientsEvent>().Publish(int.Parse(GenerateClientsNr));
                    if (_store != null)
                    {
                        WaitingClients.Clear();
                        WaitingClients.AddRange(_store.Clients);
                    }
                }
            });

            _eventAggregator.GetEvent<LogEvent>().Subscribe(OnLog);
            _eventAggregator.GetEvent<GenerateClientsCompletedEvent>().Subscribe(OnGenerate);
        }

        ~MainWindowViewModel()
        {
            _eventAggregator.GetEvent<LogEvent>().Unsubscribe(OnLog);
            _eventAggregator.GetEvent<GenerateClientsCompletedEvent>().Unsubscribe(OnGenerate);
        }

        private void OnStart()
        {
            Reset();

            _store = new Store(_eventAggregator);

            WaitingClients.AddRange(_store.Clients);

            new Thread(() =>
            {
                var queues = new ConcurrentQueue<ObservableCollection<Client>>();
                new List<ObservableCollection<Client>> { Q1, Q2, Q3, Q4 }.ForEach(x => queues.Enqueue(x));

                foreach (var storeServiceProcessor in _store.ServiceProcessors)
                {
                    if (queues.TryDequeue(out var q))
                    {
                        _mapper[storeServiceProcessor.QueueGuid] = q;
                    }
                }

                _store.Process();
            }).Start();
        }

        private void OnGenerate()
        {
            if (_store != null)
            {
                WaitingClients.Clear();
                WaitingClients.AddRange(_store.Clients);
            }
        }

        private void OnLog(Log log)
        {
            var queue = _mapper[log.QueueGuid];

            Application.Current?.Dispatcher?.Invoke(() =>
            {
                queue.Add(log.Client);

                WaitingClients.Remove(log.Client);

                Log += $"{log.Message}{Environment.NewLine}";
            });
        }

        private void OnStop()
        {
            _eventAggregator.GetEvent<StopEvent>().Publish();
        }

        private void Reset()
        {
            WaitingClients.Clear();

            foreach (var keyValuePair in _mapper)
            {
                keyValuePair.Value.Clear();
            }

            Log = string.Empty;

            _mapper.Clear();
        }
    }
}
