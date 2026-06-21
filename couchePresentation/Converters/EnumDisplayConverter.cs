using System.Globalization;
using System.Windows;
using System.Windows.Data;
using RefugeAnimaux.classesMetier;

namespace RefugeAnimaux.couchePresentation.Converters;

public class EnumDisplayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            TypeAnimal typeAnimal => typeAnimal.ToDisplay(),
            Sexe sexe => sexe.ToDisplay(),
            TypeCompatibilite typeCompatibilite => typeCompatibilite.ToDisplay(),
            ValeurCompatibilite valeurCompatibilite => valeurCompatibilite.ToDisplay(),
            StatutAdoption statutAdoption => statutAdoption.ToDisplay(),
            TypeContact typeContact => typeContact.ToDisplay(),
            _ => value?.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}
