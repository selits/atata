﻿using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading;

namespace Atata
{
    public abstract class PageObject<T> : UIComponent<T>
        where T : PageObject<T>
    {
        protected PageObject()
        {
            NavigateOnInit = true;
            ScopeLocator = new DynamicScopeLocator(GetScope);
        }

        public bool NavigateOnInit { get; set; }

        protected PageObjectContext PageObjectContext { get; private set; }

        protected UIComponent PreviousPageObject { get; private set; }

        protected virtual IWebElement GetScope(SearchOptions searchOptions)
        {
            return Driver.Get(By.TagName("body").With(searchOptions));
        }

        internal void Init(PageObjectContext context)
        {
            PageObjectContext = context;

            ApplyContext(context);
            LogGoTo();

            OnInit();

            if (NavigateOnInit)
                Navigate();

            Wait(2);

            InitComponent();

            Log.EndSection();

            RunTriggers(TriggerEvents.OnPageObjectInit);

            VerifyCurrentPage();
        }

        protected virtual void ApplyContext(PageObjectContext context)
        {
            Log = context.Logger;
            Driver = context.Driver;
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void Navigate()
        {
            NavigateToAttribute attribute;

            if (GetType().TryGetCustomAttribute(out attribute, true))
            {
                Log.Info("Navigate to {0}", attribute.Url);
                Navigate(attribute.Url);
            }
        }

        protected virtual void Navigate(string url)
        {
            Driver.Navigate().GoToUrl(url);
        }

        private void VerifyCurrentPage()
        {
            VerifyContainsContent();
            OnVerify();
        }

        private void VerifyContainsContent()
        {
            ContentContainsAttribute attribute;

            if (GetType().TryGetCustomAttribute(out attribute) && attribute.Values != null)
            {
                string[] values = attribute.Values != null ? attribute.Values.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray() : new string[0];
                string notFoundValue = null;
                bool hasContent = Scope.Try().Until(x =>
                    {
                        string text = x.Text;
                        notFoundValue = attribute.Values.FirstOrDefault(value => !text.Contains(value));
                        return notFoundValue == null;
                    });
                Assert.That(hasContent, "Expected to find content: {0}", notFoundValue);
            }
        }

        protected virtual void OnVerify()
        {
        }

        protected void Wait(double seconds)
        {
            Thread.Sleep((int)(seconds * 1000));
        }

        private void LogGoTo()
        {
            string pageObjectName = UIComponentResolver.ResolvePageObjectName<T>();
            Log.StartSection("Go to {0}", pageObjectName);
        }

        protected TOther InitChild<TOther>(string windowName = null) where TOther : PageObject<TOther>, new()
        {
            return InitChild(new TOther(), windowName);
        }

        protected TOther InitChild<TOther>(TOther pageObject, string windowName = null) where TOther : PageObject<TOther>
        {
            RunTriggers(TriggerEvents.OnPageObjectLeave);

            PageObjectContext context = PageObjectContext;

            if (!string.IsNullOrWhiteSpace(windowName))
            {
                context = SwitchTo(windowName);
            }

            pageObject.PreviousPageObject = this;
            pageObject.Init(context);

            return pageObject;
        }

        protected virtual PageObjectContext SwitchTo(string windowName)
        {
            Driver.SwitchTo().Window(windowName);
            return PageObjectContext;
        }

        public TOther GoTo<TOther>() where TOther : PageObject<TOther>
        {
            TOther newPageObject = Activator.CreateInstance<TOther>();
            return GoTo(newPageObject);
        }

        public TOther GoTo<TOther>(TOther pageObject) where TOther : PageObject<TOther>
        {
            pageObject.NavigateOnInit = false;
            return InitChild(pageObject);
        }

        public TOther GoToNewWindow<TOther>() where TOther : PageObject<TOther>
        {
            TOther newPageObject = Activator.CreateInstance<TOther>();
            return GoToNewWindow(newPageObject);
        }

        public TOther GoToNewWindow<TOther>(TOther pageObject) where TOther : PageObject<TOther>
        {
            pageObject.NavigateOnInit = false;
            string windowHandle = Driver.WindowHandles.SkipWhile(x => x != Driver.CurrentWindowHandle).ElementAt(1);
            return InitChild(pageObject, windowHandle);
        }

        // TODO: Change return type to self. Remove or implement SetAllGeneratables method.
        ////public void SetAllGeneratables()
        ////{
        ////}

        public virtual void CloseWindow()
        {
            Driver.ExecuteScript("window.close();");
        }

        public T Do<TComponent>(Func<T, TComponent> childControlGetter, params Action<TComponent>[] actions)
            where TComponent : Control<T>
        {
            TComponent component = childControlGetter((T)this);

            foreach (var action in actions)
                action(component);

            return (T)this;
        }
    }
}