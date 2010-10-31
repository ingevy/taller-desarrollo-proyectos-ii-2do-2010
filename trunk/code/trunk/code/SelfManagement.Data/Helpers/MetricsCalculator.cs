namespace CallCenter.SelfManagement.Data.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MetricsCalculator
    {
        public double CalculateAcumulatedMetricValue(IList<UserMetric> userMetrics, DateTime date)
        {
            var metricValue = 0.0;

            var maxDateWithData = (from u in userMetrics
                                   select u.Date).ToList().Max();

            if (date <= maxDateWithData)
            {
                if (userMetrics.Count > 0)
                {
                    foreach (var um in userMetrics)
                    {
                        metricValue += um.Value;
                    }
                }
            }
            else
            {
                if (userMetrics.Count > 0)
                {
                    var solvr = new LeastSquareQuadraticRegression();
                    solvr.AddPoints(Convert.ToDouble(0), Convert.ToDouble(0));
                    var maxY = 0.0;

                    foreach (var um in userMetrics)
                    {
                        metricValue += um.Value;
                        maxY = (um.Value > maxY) ? um.Value : maxY;
                        solvr.AddPoints(Convert.ToDouble(um.Date.Day), metricValue);
                    }

                    metricValue = solvr.calculatePredictedY(Convert.ToDouble(date.Day));
                    metricValue = (metricValue >= maxY) ? metricValue : maxY;
                }
            }

            return metricValue;
        }

        public double CalculateAverageMetricValue(IList<UserMetric> userMetrics, DateTime date)
        {
            var metricValue = 0.0;

            var maxDateWithData = (from u in userMetrics
                                   select u.Date).ToList().Max();

            if (date <= maxDateWithData)
            {
                if (userMetrics.Count > 0)
                {
                    foreach (var um in userMetrics)
                    {
                        metricValue += um.Value;
                    }

                    metricValue = metricValue / Convert.ToDouble(userMetrics.Count);
                }
            }
            else
            {
                if (userMetrics.Count > 0)
                {
                    var solvr = new LeastSquareQuadraticRegression();
                    solvr.AddPoints(Convert.ToDouble(0), Convert.ToDouble(0));

                    foreach (var um in userMetrics)
                    {
                        metricValue += um.Value;
                        solvr.AddPoints(Convert.ToDouble(um.Date.Day), um.Value);
                    }

                    for (var i = maxDateWithData.Day + 1; i <= date.Day; i++)
                    {
                        metricValue += solvr.calculatePredictedY(Convert.ToDouble(Convert.ToDouble(i)));
                    }

                    metricValue = metricValue / (Convert.ToDouble(userMetrics.Count) + Convert.ToDouble(date.Day - maxDateWithData.Day));
                }
            }

            if (metricValue < 0) { metricValue = 0.0; }
            if (metricValue > 100) { metricValue = 100.0; }

            return metricValue;
        }
    }
}
