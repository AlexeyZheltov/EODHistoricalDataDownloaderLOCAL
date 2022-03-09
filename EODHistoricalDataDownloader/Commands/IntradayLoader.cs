﻿using EOD;
using EOD.Model;

using EODHistoricalDataDownloader.Model;

using EODLoader.Services.Utils;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using static EOD.API;

namespace EODHistoricalDataDownloader.Commands
{
    internal class IntradayLoader
    {
        internal string ApiKey { get; set; }
        internal List<LoadingStatus> LoadingStatuses { get; set; }
        internal IntradayHistoricalInterval Interval { get; set; }
        internal DateTime DateFrom { get; set; }
        internal DateTime DateTo { get; set; }
        internal int MaxThreads { get; set; }
        internal WebProxy Proxy { get; set; }
        internal bool IsUpdate { get; set; }

        internal IntradayLoader(string apiKey, List<LoadingStatus> loadingStatuses, IntradayHistoricalInterval interval, DateTime dateFrom, DateTime dateTo,
            int maxThreads, WebProxy proxy, bool isUpdate)
        {
            ApiKey = apiKey;
            LoadingStatuses = loadingStatuses;
            Interval = interval;
            DateFrom = dateFrom;
            DateTo = dateTo;
            MaxThreads = maxThreads;
            Proxy = proxy;
            IsUpdate = isUpdate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        internal void LoadToCsv(string filePath, CancellationTokenSource? source = null)
        {
            if (source == null)
            {
                source = new CancellationTokenSource();
            }
            try
            {
                var queue = new Queue<LoadingStatus>(LoadingStatuses);
                var _api = new API(ApiKey, Proxy, Program.Program.ProgramName);
                IUtilsService utils = new UtilsService();
                List<Task> tasks = new();
                while (queue.TryPeek(out LoadingStatus _))
                {
                    LoadingStatus status = queue.Dequeue();

                    var reqSave = Task.Factory.StartNew(async () =>
                    {
                        try
                        {
                            status.Status = "Processing";
                            List<IntradayHistoricalStockPrice>? response = _api.GetIntradayHistoricalStockPriceAsync(status.Ticker, DateFrom, DateTo, Interval).Result;
                            string path = $@"{filePath}\{status.Ticker}.csv";
                            await utils.CreateCVSFile(response, path, IsUpdate);
                            status.Status = "OK";
                            status.Filename = path;
                        }
                        catch (Exception ex)
                        {
                            status.Status = "Error";
                            status.Filename = ex.Message;
                        }
                    }, source.Token);
                    tasks.Add(reqSave);

                    if (tasks.Count == MaxThreads)
                    {
                        int i = Task.WaitAny(tasks.ToArray());
                        tasks.Remove(tasks[i]);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
