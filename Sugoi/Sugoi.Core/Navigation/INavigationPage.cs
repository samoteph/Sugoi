using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core.Navigation
{
    public interface INavigationPage : IPage
    {
        /// <summary>
        /// Navigation dans la page appelé à partir de GameNavigation.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="parameter"></param>
        /// <returns>si state == Ending + return à false permet de stopper la navigation. Sans effet pour un autre state</returns>
        bool Navigate(NavigationStates state, object parameter);
    }

    public enum NavigationStates
    {
        Starting,
        Started,
        Ending,
        Ended
    }
}
