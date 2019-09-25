using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace Evect.Models
{
    public class KeepAlivedHostedService : IHostedService, IDisposable
    {

        private Timer _timer;
        private readonly ILogger<KeepAlivedHostedService> _logger;

        public KeepAlivedHostedService(ILogger<KeepAlivedHostedService> logger)
        {
            _logger = logger;
            _timer = new Timer(60000);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }
        
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                WebRequest req = WebRequest.Create("https://bot.diffind.com");
                req.GetResponse();
                _logger.LogInformation("Created request");
            }
            catch
            {
                _logger.LogInformation("Someting went wrong");
            }
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timer started");

            _timer.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Stop();
            _logger.LogInformation("Timer stoped");
            return Task.CompletedTask;

        }

        public void Dispose()
        {
            _timer.Dispose();
            throw new NotImplementedException();
        }
    }
}