using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace dotnet_sqs_sample
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IAmazonSQS _sqs;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IAmazonSQS sqs, IConfiguration configuration)
        {
            _logger = logger;
            _sqs = sqs;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var request = new ReceiveMessageRequest
                    {
                        QueueUrl = _configuration.GetSection("QueueUrl").Value,
                        MaxNumberOfMessages = 1,
                        WaitTimeSeconds = 5
                    };

                    var result = await _sqs.ReceiveMessageAsync(request, stoppingToken);
                    if (result.Messages.Any())
                    {
                        foreach (var message in result.Messages)
                        {
                            // Some Processing code would live here
                            _logger.LogInformation("Processing Message: {message} | {time}", message.Body, DateTimeOffset.Now);

                            var processedMessage = new ProcessedMessage(message.Body);

                            //TODO: Implement splash and resubmit new url to re-index elasticsearch
                            // var sendRequest = new SendMessageRequest(_queueProcessed, JsonConvert.SerializeObject(processedMessage));
                            // var sendResult = await _sqs.SendMessageAsync(sendRequest, stoppingToken);
                            // if (sendResult.HttpStatusCode == System.Net.HttpStatusCode.OK)
                            // {
                                // var deleteResult = await _sqs.DeleteMessageAsync(_exampleQueueUrl, message.ReceiptHandle);
                            // }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.InnerException?.ToString());
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                
            }
        }
    }
    
    public class ProcessedMessage
    {
        public ProcessedMessage(string message, bool hasErrors = false)
        {
            TimeStamp = DateTime.UtcNow;

            Message = message;
            HasErrors = hasErrors;
        }

        public DateTime TimeStamp { get; set; }
        public string Message { get; set; }
        public bool HasErrors { get; set; }
    }
}