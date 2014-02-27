using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Raven.TestSuite.Client.Wpf.ViewModels.TestsComparator
{
    public class TestComparisonItemViewModel
    {
        public string Name { get; set; }

        public TimeSpan? LeftExecutionTime { get; set; }

        public TimeSpan? RightExecutionTime { get; set; }

        public double? DifferenceRatio
        {
            get
            {
                if (LeftExecutionTime.HasValue && RightExecutionTime.HasValue)
                {
                    return
                        RightExecutionTime.Value.TotalMilliseconds / LeftExecutionTime.Value.TotalMilliseconds;
                }
                return null;
            }
        }

        public Brush Color
        {
            get
            {
                if (!DifferenceRatio.HasValue || DifferenceRatio.Value.Equals(1))
                {
                    return Brushes.DarkGray;
                }
                    return DifferenceRatio.Value < 1 ? Brushes.PaleGreen : Brushes.OrangeRed;
            }
        }

        public string PercentageDifference
        {
            get
            {
                if (DifferenceRatio.HasValue)
                {
                    return DifferenceRatio.Value.ToString("P");
                }
                return string.Empty;
            }
        }
    }
}
