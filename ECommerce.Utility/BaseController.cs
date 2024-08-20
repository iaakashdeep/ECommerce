using ECommerce.Utility.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Utility
{
    public class BaseExceptionController:ExceptionFilterAttribute
    {
        private ILogger<BaseExceptionController> _logger;
        private LoggerSingleton _loggerSingleton=LoggerSingleton.getInstance;
        
        public BaseExceptionController(ILogger<BaseExceptionController> logger)
        {
            _logger = logger;
            

        }

        public override void OnException(ExceptionContext context)
        {
            _loggerSingleton.Log(context.Exception.ToString());
            context.ExceptionHandled = true;

        }
    }
}
