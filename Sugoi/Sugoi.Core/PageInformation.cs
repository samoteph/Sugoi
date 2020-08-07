using System;
using System.Runtime.InteropServices.ComTypes;

namespace Sugoi.Core.Navigation
{
    /// <summary>
    /// Information sur la page naviguéé
    /// </summary>

    public class PageInformation
    {
        public PageInformation(Type typePage, object parameter, INavigationPage page)
        {
            this.TypePage = typePage;
            this.Parameter = parameter;
            this.Page = page;
        }

        public bool Navigate(NavigationStates state)
        {
            return this.Page.Navigate(state, this.Parameter);
        }

        public INavigationPage Page
        {
            get;
            private set;
        }

        public Type TypePage
        {
            get;
            private set;
        }

        public object Parameter
        {
            get;
            private set;
        }
    }
}