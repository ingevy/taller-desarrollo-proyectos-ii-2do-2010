namespace CallCenter.SelfManagement.Data.Helpers
{
    using System;
    using System.Collections.Generic;

    public class MetricsCalculator
    {
        public double CalculateAcumulatedMetricValue(IList<UserMetric> userMetrics, DateTime date)
        {
            var metricValue = 0.0;

            if (date <= DateTime.Now)
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

                    foreach (var um in userMetrics)
                    {
                        metricValue += um.Value;
                        solvr.AddPoints(Convert.ToDouble(um.Date.Day), metricValue);
                    }

                    metricValue = solvr.calculatePredictedY(Convert.ToDouble(date.Day));
                }
            }

            return metricValue;
        }

        public double CalculateAverageMetricValue(IList<UserMetric> userMetrics, DateTime date)
        {
            var metricValue = 0.0;

            if (date <= DateTime.Now)
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

                    for (var i = DateTime.Now.Day + 1; i <= date.Day; i++)
                    {
                        metricValue += solvr.calculatePredictedY(Convert.ToDouble(Convert.ToDouble(i)));
                    }

                    metricValue = metricValue / (Convert.ToDouble(userMetrics.Count) + Convert.ToDouble(date.Day - DateTime.Now.Day));
                }
            }

            return metricValue;
        }
    }
}
