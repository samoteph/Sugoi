using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core.Navigation
{
    public class GameNavigation
    {
        // Historique des types de pages naviguées
        private Stack<PageInformation> historyPages = new Stack<PageInformation>(20);

        public Machine Machine
        {
            get;
            private set;
        }

        public GameNavigation(Machine machine)
        {
            this.Machine = machine;
            this.Pages = new Dictionary<Type, INavigationPage>(10);
        }

        /// <summary>
        /// Type de la page navigué
        /// </summary>
        public PageInformation CurrentPageInformation
        {
            get;
            private set;
        }

        /// <summary>
        /// Pages
        /// </summary>
        Dictionary<Type, INavigationPage> Pages
        {
            get;
            set;
        }

        /// <summary>
        /// Ajouter une page
        /// </summary>
        /// <typeparam name="TPage"></typeparam>
        /// <returns></returns>

        public TPage AddPage<TPage>() where TPage : INavigationPage
        {
            // ajout des pages
            TPage page = Activator.CreateInstance<TPage>(); 
            Pages.Add(typeof(TPage), page);

            return page;
        }

        /// <summary>
        /// Retirer la page
        /// </summary>
        /// <typeparam name="TPage"></typeparam>
        /// <returns></returns>

        public bool RemovePage<TPage>() where TPage : INavigationPage
        {
            if (Pages.ContainsKey(typeof(TPage)))
            {
                Pages.Remove(typeof(TPage));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creation d'une page d'information
        /// </summary>
        /// <param name="typePage"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>

        private PageInformation CreatePageInformation(Type typePage, object parameter)
        {
            var page = Pages[typePage];
            return new PageInformation(typePage, parameter, page);
        }

        /// <summary>
        /// Permet de creer la pageInformation et de la pousser dans l'historique (mais pas de l'affecter dans CurrentPageInformation)
        /// </summary>
        /// <param name="typePage"></param>
        /// <returns></returns>

        private void PushPageInformation(PageInformation pageInformation)
        {
            if(pageInformation != null)
            {
                this.historyPages.Push(pageInformation);
            }
        }

        private PageInformation PopPageInformation()
        {
            if (CanGoBack)
            {
                var pageInformation = this.historyPages.Pop();
                return pageInformation;
            }

            return null;
        }

        private PageInformation PeekPageInformation()
        {
            if (CanGoBack)
            {
                var pageInformation = this.historyPages.Peek();
                return pageInformation;
            }

            return null;
        }

        public bool CanGoBack
        {
            get
            {
                return this.historyPages.Count > 0;
            }
        }

        public INavigationPage Navigate(Type typeOfPage, object parameter = null)
        {
            if (this.IsNavigating == true)
            {
                return null; ;
            }

            this.IsNavigating = true;

            try
            {
                // On demande à l'ancienne page si elle est OK pour quitter
                if (CurrentPageInformation != null)
                {
                    var canNavigate = this.CurrentPageInformation.Navigate(NavigationStates.Ending);
                
                    if(canNavigate == false)
                    {
                        return null;
                    }

                    this.CurrentPageInformation.Navigate(NavigationStates.Ended);
                }

                // Stocke l'ancienne page
                this.PushPageInformation(this.CurrentPageInformation);

                // crée la nouvelle
                var pageInformation = CreatePageInformation(typeOfPage, parameter);
                this.CurrentPageInformation = pageInformation;

                pageInformation.Navigate(NavigationStates.Starting);

                var page = pageInformation.Page;

                page.Initialize();

                this.Machine.InitializeCallback = null;
                this.Machine.UpdatingCallback = page.Updating;
                this.Machine.UpdatedCallback = page.Updated;
                this.Machine.DrawCallback = page.Draw;

                pageInformation.Navigate(NavigationStates.Started);

                return page;
            }
            finally
            {
                this.IsNavigating = false;
            }
        }

        /// <summary>
        /// Navigation sans transition
        /// </summary>
        /// <param name="typePage"></param>
        /// <returns></returns>

        public IPage Navigate<TPage>(object parameter = null) where TPage : IPage
        {
            return this.Navigate(typeof(TPage), parameter);
        }

        /// <summary>
        /// Retour sans transition
        /// </summary>
        /// <param name="typePage"></param>
        /// <returns></returns>

        public INavigationPage GoBack()
        {
            if (this.IsNavigating == true)
            {
                return null; ;
            }

            this.IsNavigating = true;

            try
            {

                // On demande à l'ancienne page si elle est OK pour quitter
                if (CurrentPageInformation != null)
                {
                    var canNavigate = this.CurrentPageInformation.Navigate(NavigationStates.Ending);

                    if (canNavigate == false)
                    {
                        return null;
                    }

                    this.CurrentPageInformation.Navigate(NavigationStates.Ended);
                }

                var pageInformation = this.PopPageInformation();

                CurrentPageInformation = pageInformation;

                pageInformation.Navigate(NavigationStates.Starting);

                var page = pageInformation.Page;

                page.Initialize();

                this.Machine.InitializeCallback = null;
                this.Machine.UpdatingCallback = page.Updating;
                this.Machine.UpdatedCallback = page.Updated;
                this.Machine.DrawCallback = page.Draw;

                pageInformation.Navigate(NavigationStates.Started);

                return page;
            }
            finally
            {
                this.IsNavigating = false;
            }
        }

        public bool IsNavigating
        {
            get;
            private set;
        }

        public PageInformation NavigateWithFade(Type typeOfPage, object parameter = null, double fadeSpeed = 1, Action completed = null)
        {
            if (this.IsNavigating == true)
            {
                return null; ;
            }

            this.IsNavigating = true;

            var oldPageInformation = this.CurrentPageInformation;
            var pageInformation = CreatePageInformation(typeOfPage, parameter);

            // navigation avec Fade
            bool canNavigate = this.InternalNavigationWithFade(pageInformation, fadeSpeed, () =>
            {
                this.PushPageInformation(oldPageInformation);
                this.IsNavigating = false;
            });

            // State Ending n'autorise pas le départ
            if(canNavigate == false)
            {
                this.IsNavigating = false;
                return null;
            }

            return pageInformation;
        }

        /// <summary>
        /// Affichage de la navigation avec fade
        /// </summary>
        /// <param name="typePage"></param>
        /// <returns></returns>

        public PageInformation NavigateWithFade<TPage>(object parameter = null, double fadeSpeed = 1, Action completed = null) where TPage : IPage
        {
            return this.NavigateWithFade(typeof(TPage), parameter, fadeSpeed, completed);
        }

        /// <summary>
        /// Retour de navigation avec transition Fade
        /// </summary>
        /// <param name="fadeSpeed"></param>
        /// <returns></returns>

        public PageInformation GoBackWithFade(double fadeSpeed = 1, Action completed = null)
        {
            if (this.IsNavigating == true)
            {
                return null;
            }

            this.IsNavigating = true;

            PageInformation pageInformation = null;

            if (CanGoBack)
            {
                pageInformation = this.PeekPageInformation();

                // navigation avec Fade
                bool canNavigate = this.InternalNavigationWithFade(pageInformation, fadeSpeed, () =>
                {
                    completed?.Invoke();

                    this.PopPageInformation();
                    this.IsNavigating = false;
                });

                // State Ending n'autorise pas le départ
                if (canNavigate == false)
                {
                    this.IsNavigating = false;
                    return null;
                }
            }

            return pageInformation;
        }

        /// <summary>
        /// Permet une navigation pour Navigate et GoBack
        /// </summary>
        /// <param name="page"></param>
        /// <param name="fadeSpeed"></param>
        /// <param name="completed"></param>

        protected bool InternalNavigationWithFade(PageInformation pageInformation, double fadeSpeed = 1, Action completed = null)
        {
            var oldDrawCallback = this.Machine.DrawCallback;

            double opacity = 0;

            // ancienne pageInformation
            var oldPageInformation = this.CurrentPageInformation;

            // on avertie l'ancienne page du départ
            bool canNavigate = oldPageInformation.Navigate(NavigationStates.Ending);

            // l'ancienne page donne son accord ?
            if(canNavigate == false)
            {
                return false;
            }

            // Draw (on continue d'afficher l'ancienne frame mais on dessine par dessus un rectangle avec opacity)
            this.Machine.DrawCallback = (frameExecuted) =>
            {
                oldDrawCallback?.Invoke(frameExecuted);

                var screen = this.Machine.Screen;

                opacity += (0.05 * fadeSpeed);

                screen.Opacity = opacity;
                screen.Clear(Argb32.Black);
                screen.Opacity = 1;

                // fin du fade in, on ne voit plus que le rectangle noir
                if (opacity >= 1)
                {
                    opacity = 1;

                    // on indique à l'ancienne page que c'est la fin
                    oldPageInformation.Navigate(NavigationStates.Ended);

                    CurrentPageInformation = pageInformation;

                    // on indique à la nouvelle page que l'on commence la navigation
                    pageInformation.Navigate(NavigationStates.Starting);

                    var page = pageInformation.Page;

                    page.Initialize();

                    this.Machine.InitializeCallback = null;
                    this.Machine.UpdatingCallback = page.Updating;
                    this.Machine.UpdatedCallback = page.Updated;
                    this.Machine.DrawCallback = (frameExecuted2) =>
                    {
                        // on affiche la nouvelle page avec le rectangle en opacity
                        page.Draw(frameExecuted2);

                        opacity -= (0.05 * fadeSpeed);

                        screen.Opacity = opacity;
                        screen.Clear(Argb32.Black);
                        screen.Opacity = 1;

                        // plus de rectangle / fin du fade
                        if (opacity <= 0)
                        {
                            this.Machine.DrawCallback = page.Draw;

                            // fin de la navigation (le fade est terminée)
                            pageInformation.Navigate(NavigationStates.Started);

                            completed?.Invoke();
                        }
                    };
                }
            };

            return true;
        }
    }
}
