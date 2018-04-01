using System.Windows;

namespace locbamlUI
{
    //According to: http://www.thomaslevesque.com/tag/datacontext/
    public class BindingProxy: Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        public object Data
        {
            get { return (object) GetValue(DataProperty);}
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));

    }
}
