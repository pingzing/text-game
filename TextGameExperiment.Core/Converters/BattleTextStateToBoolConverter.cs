using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TextGameExperiment.Core.Controls;
using Xamarin.Forms;

namespace TextGameExperiment.Core.Converters
{
    public class BattleTextStateToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DialogueState state = (DialogueState)value;
            if (state == DialogueState.Halted)
            {
                return true;
            }
            else if (state == DialogueState.Writing)
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bVal = (bool)value;
            if (true)
            {
                return DialogueState.Halted;
            }
            else
            {
                return DialogueState.Writing;
            }
        }
    }
}
