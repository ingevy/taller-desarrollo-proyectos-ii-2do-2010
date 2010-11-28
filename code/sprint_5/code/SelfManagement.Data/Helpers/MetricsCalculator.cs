namespace CallCenter.SelfManagement.Data.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class MetricsCalculator
    {
        public static double CalculateAcumulatedMetricValue(IList<UserMetric> userMetrics, DateTime date)
        {
            var metricValue = 0.0;
            var dates = from u in userMetrics select u.Date;
            var maxDateWithData = (dates.Count() != 0) ? dates.Max() : date;
            var acumulativeMetrics = (from u in userMetrics
                                      where u.Date <= date
                                      select u).ToList();

            if (date <= maxDateWithData)
            {
                if (acumulativeMetrics.Count > 0)
                {
                    foreach (var um in acumulativeMetrics)
                    {
                        metricValue += um.Value;
                    }
                }
            }
            else
            {
                if (acumulativeMetrics.Count > 0)
                {
                    var solvr = new LeastSquareQuadraticRegression();
                    solvr.AddPoints(Convert.ToDouble(0), Convert.ToDouble(0));
                    var maxY = 0.0;

                    foreach (var um in acumulativeMetrics)
                    {
                        metricValue += um.Value;
                        maxY = (um.Value > maxY) ? um.Value : maxY;
                        solvr.AddPoints(Convert.ToDouble(um.Date.Day), metricValue);
                    }

                    try
                    {
                        metricValue = solvr.CalculatePredictedY(Convert.ToDouble(date.Day));
                    }
                    catch
                    {
                        metricValue = maxY;
                    }
                    
                    metricValue = (metricValue >= maxY) ? metricValue : maxY;
                }
            }

            return metricValue;
        }

        public static double CalculateAverageMetricValue(IList<UserMetric> userMetrics, DateTime date, int metricFormat)
        {
            var metricValue = 0.0;
            var dates = from u in userMetrics select u.Date;
            var maxDateWithData = (dates.Count() != 0) ? dates.Max() : date;
            var acumulativeMetrics = (from u in userMetrics
                                      where u.Date <= date
                                      select u).ToList();

            if (date <= maxDateWithData)
            {
                if (acumulativeMetrics.Count > 0)
                {
                    foreach (var um in acumulativeMetrics)
                    {
                        metricValue += um.Value;
                    }

                    metricValue = metricValue / Convert.ToDouble(acumulativeMetrics.Count);
                }
            }
            else
            {
                if (acumulativeMetrics.Count > 0)
                {
                    var solvr = new LeastSquareQuadraticRegression();
                    solvr.AddPoints(Convert.ToDouble(0), Convert.ToDouble(0));
                    var maxData = 0.0;
                    var minData = double.MaxValue;

                    foreach (var um in acumulativeMetrics)
                    {
                        metricValue += um.Value;
                        maxData = (um.Value > maxData) ? um.Value : maxData;
                        minData = (um.Value < minData) ? um.Value : minData;
                        solvr.AddPoints(Convert.ToDouble(um.Date.Day), um.Value);
                    }

                    for (var i = maxDateWithData.Day + 1; i <= date.Day; i++)
                    {
                        try
                        {
                            metricValue += solvr.CalculatePredictedY(Convert.ToDouble(Convert.ToDouble(i)));
                        }
                        catch
                        {
                            metricValue += ((maxData + minData) / Convert.ToDouble(2));
                        }
                    }

                    metricValue = metricValue / (Convert.ToDouble(acumulativeMetrics.Count) + Convert.ToDouble(date.Day - maxDateWithData.Day));
                }
            }

            if (metricFormat == 0)
            {
                if (metricValue < 0) { metricValue = 0.0; }
                if (metricValue > 100) { metricValue = 100.0; }
            }
            else if (metricFormat == 2)
            {
                if (metricValue < 0) { metricValue = 0.0; }
            }

            return metricValue;
        }

        public static double CalculateAcumulatedMetricValue(List<CampaingMetric> campaingMetrics, DateTime date)
        {
            var metricValue = 0.0;
            var dates = from u in campaingMetrics select u.Date;
            var maxDateWithData = (dates.Count() != 0) ? dates.Max() : date;
            var acumulativeMetrics = (from u in campaingMetrics
                                      where u.Date <= date
                                      select u).ToList();

            if (date <= maxDateWithData)
            {
                if (acumulativeMetrics.Count > 0)
                {
                    foreach (var um in acumulativeMetrics)
                    {
                        metricValue += um.Value;
                    }
                }
            }
            else
            {
                if (acumulativeMetrics.Count > 0)
                {
                    var solvr = new LeastSquareQuadraticRegression();
                    solvr.AddPoints(Convert.ToDouble(0), Convert.ToDouble(0));
                    var maxY = 0.0;

                    foreach (var um in acumulativeMetrics)
                    {
                        metricValue += um.Value;
                        maxY = (um.Value > maxY) ? um.Value : maxY;
                        solvr.AddPoints(Convert.ToDouble(um.Date.Day), metricValue);
                    }

                    try
                    {
                        metricValue = solvr.CalculatePredictedY(Convert.ToDouble(date.Day));
                    }
                    catch
                    {
                        metricValue = maxY;
                    }
                    
                    metricValue = (metricValue >= maxY) ? metricValue : maxY;
                }
            }

            return metricValue;
        }

        public static double CalculateAverageMetricValue(List<CampaingMetric> campaingMetrics, DateTime date, int metricFormat)
        {
            var metricValue = 0.0;
            var dates = from u in campaingMetrics select u.Date;
            var maxDateWithData = (dates.Count() != 0) ? dates.Max() : date;
            var acumulativeMetrics = (from u in campaingMetrics
                                      where u.Date <= date
                                      select u).ToList();

            if (date <= maxDateWithData)
            {
                if (acumulativeMetrics.Count > 0)
                {
                    foreach (var um in acumulativeMetrics)
                    {
                        metricValue += um.Value;
                    }

                    metricValue = metricValue / Convert.ToDouble(acumulativeMetrics.Count);
                }
            }
            else
            {
                if (acumulativeMetrics.Count > 0)
                {
                    var solvr = new LeastSquareQuadraticRegression();
                    solvr.AddPoints(Convert.ToDouble(0), Convert.ToDouble(0));
                    var maxData = 0.0;
                    var minData = double.MaxValue;

                    foreach (var um in acumulativeMetrics)
                    {
                        metricValue += um.Value;
                        maxData = (um.Value > maxData) ? um.Value : maxData;
                        minData = (um.Value < minData) ? um.Value : minData;
                        solvr.AddPoints(Convert.ToDouble(um.Date.Day), um.Value);
                    }

                    for (var i = maxDateWithData.Day + 1; i <= date.Day; i++)
                    {
                        try
                        {
                            metricValue += solvr.CalculatePredictedY(Convert.ToDouble(Convert.ToDouble(i)));
                        }
                        catch
                        {
                            metricValue += ((maxData + minData) / Convert.ToDouble(2));
                        }
                    }

                    metricValue = metricValue / (Convert.ToDouble(acumulativeMetrics.Count) + Convert.ToDouble(date.Day - maxDateWithData.Day));
                }
            }

            if (metricFormat == 0)
            {
                if (metricValue < 0) { metricValue = 0.0; }
                if (metricValue > 100) { metricValue = 100.0; }
            }
            else if (metricFormat == 2)
            {
                if (metricValue < 0) { metricValue = 0.0; }
            }

            return metricValue;
        }
    }
}
