using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class ChartingActor : ReceiveActor
    {
        #region Messages

        public class InitializeChart
        {
            public InitializeChart(Dictionary<string, Series> initialSeries)
            {
                InitialSeries = initialSeries;
            }

            public Dictionary<string, Series> InitialSeries { get; private set; }
        }

        public class AddSeries
        {
            public readonly Series Series;

            public AddSeries(Series series)
            {
                Series = series;
            }
        }

        public class RemoveSeries
        {
            public readonly string Name;

            public RemoveSeries(string name)
            {
                Name = name;
            }
        }

       #endregion

        private readonly Chart _chart;
        private Dictionary<string, Series> _seriesIndex;
        private const int MAX_NUMBER_OF_POINTS = 250;
        private int _xPosCounter = 0;

        public ChartingActor(Chart chart) : this(chart, new Dictionary<string, Series>())
        {
        }

        public ChartingActor(Chart chart, Dictionary<string, Series> seriesIndex)
        {
            _chart = chart;
            _seriesIndex = seriesIndex;

            Receive<InitializeChart>(initMsg => HandleInitialize(initMsg));
            Receive<AddSeries>(addCmd => HandleAddSeries(addCmd));
            Receive<RemoveSeries>(removeCmd => HandleRemoveSeries(removeCmd));
            Receive<Metric>(metric => HandleMetrics(metric));
        }

        #region Individual Message Type Handlers

        private void HandleInitialize(InitializeChart ic)
        {
            if (ic.InitialSeries != null)
            {
                _seriesIndex = ic.InitialSeries;
            }

            _chart.Series.Clear();

            var area = _chart.ChartAreas[0];
            area.AxisX.IntervalType = DateTimeIntervalType.Number;
            area.AxisY.IntervalType = DateTimeIntervalType.Number;

            SetChartBoundaries();

            if (_seriesIndex.Any())
            {
                foreach (var series in _seriesIndex)
                {
                    series.Value.Name = series.Key;
                    _chart.Series.Add(series.Value);
                }
            }

            SetChartBoundaries();
        }

        private void HandleRemoveSeries(RemoveSeries series)
        {
            if (!string.IsNullOrEmpty(series.Name) && _seriesIndex.ContainsKey(series.Name))
            {
                var seriesToRemove = _seriesIndex[series.Name];
                _seriesIndex.Remove(seriesToRemove.Name);
                _chart.Series.Remove(seriesToRemove);
                SetChartBoundaries();
            }
        }

        private void HandleMetrics(Metric metric)
        {
            if (!string.IsNullOrEmpty(metric.Series) && _seriesIndex.ContainsKey(metric.Series))
            {
                var series = _seriesIndex[metric.Series];
                series.Points.AddXY(_xPosCounter++, metric.CounterValue);
                while (series.Points.Count > MAX_NUMBER_OF_POINTS)
                {
                    series.Points.RemoveAt(0);
                }
                SetChartBoundaries();
            }
        }


        private void HandleAddSeries(AddSeries series)
        {
            if (!string.IsNullOrEmpty(series.Series.Name) && !_seriesIndex.ContainsKey(series.Series.Name))
            {
                _seriesIndex.Add(series.Series.Name, series.Series);
                _chart.Series.Add(series.Series);
                SetChartBoundaries();
            }
        }

        #endregion

        private void SetChartBoundaries()
        {
            double maxAxisX, maxAxisY, minAxisX, minAxisY = 0.0d;

            var allPoints = new HashSet<DataPoint>(
                _seriesIndex.Values.SelectMany(series=>series.Points));

            var yValues = allPoints.SelectMany(point => point.YValues).ToList();

            maxAxisX = _xPosCounter;
            minAxisX = _xPosCounter - MAX_NUMBER_OF_POINTS;
            maxAxisY = yValues.Count > 0 ? Math.Ceiling(yValues.Max()) : 1.0d;
            minAxisY = yValues.Count > 0 ? Math.Floor(yValues.Min()) : 0.0d;

            if (allPoints.Count > 2)
            {
                var area = _chart.ChartAreas[0];
                area.AxisX.Minimum = minAxisX;
                area.AxisX.Maximum = maxAxisX;
                area.AxisY.Minimum = minAxisY;
                area.AxisY.Maximum = maxAxisY;
            }
        }

    }
}
