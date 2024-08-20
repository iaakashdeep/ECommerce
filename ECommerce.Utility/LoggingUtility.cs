using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Utility
{
    //public class LoggingUtility : IHostedService
    //{
    //    /// <summary>
    //    /// This acts as a background service and will run parallely with your application, you can configure the logics in this class
    //    /// </summary>
    //    private readonly ILogger<LoggingUtility> _logger;
    //    public LoggingUtility(ILogger<LoggingUtility> logging)
    //    {
    //        _logger = logging;
    //    }
    //    public Task StartAsync(CancellationToken cancellationToken)
    //    {
    //        //_logger.LogInformation("Background Service Logging Utitlity Starting");
    //       // return Task.CompletedTask;
    //    }

    //    public Task StopAsync(CancellationToken cancellationToken)
    //    {
    //        //_logger.LogInformation("Background Service Logging Utitlity Stopping");
    //        //return Task.CompletedTask;
    //    }
    //}
}
