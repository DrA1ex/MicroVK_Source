using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace MicroVK.CSharp.Controls
{
    public static class ParentOfTypeExtensions
    {
        public static T ParentOfType<T>(this DependencyObject element) where T : DependencyObject
        {
            if (element == null)
                return default(T);
            return
                Enumerable.FirstOrDefault<T>(
                    Enumerable.OfType<T>((IEnumerable) ParentOfTypeExtensions.GetParents(element)));
        }

        public static bool IsAncestorOf(this DependencyObject element, DependencyObject descendant)
        {
            ArgumentVerificationExtensions.TestNotNull((object) element, "element");
            ArgumentVerificationExtensions.TestNotNull((object) descendant, "descendant");
            if (descendant != element)
                return Enumerable.Contains<DependencyObject>(ParentOfTypeExtensions.GetParents(descendant), element);
            return true;
        }

        public static T GetVisualParent<T>(this DependencyObject element) where T : DependencyObject
        {
            return ParentOfTypeExtensions.ParentOfType<T>(element);
        }

        internal static IEnumerable<T> GetAncestors<T>(this DependencyObject element) where T : class
        {
            return Enumerable.OfType<T>((IEnumerable) ParentOfTypeExtensions.GetParents(element));
        }

        internal static T GetParent<T>(this DependencyObject element) where T : FrameworkElement
        {
            return ParentOfTypeExtensions.ParentOfType<T>(element);
        }

        public static IEnumerable<DependencyObject> GetParents(this DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            while ((element = ParentOfTypeExtensions.GetParent(element)) != null)
                yield return element;
        }

        private static DependencyObject GetParent(this DependencyObject element)
        {
            DependencyObject dependencyObject;
            try
            {
                dependencyObject = VisualTreeHelper.GetParent(element);
            }
            catch (InvalidOperationException ex)
            {
                dependencyObject = (DependencyObject) null;
            }
            if (dependencyObject == null)
            {
                FrameworkElement frameworkElement = element as FrameworkElement;
                if (frameworkElement != null)
                    dependencyObject = frameworkElement.Parent;
                FrameworkContentElement frameworkContentElement = element as FrameworkContentElement;
                if (frameworkContentElement != null)
                    dependencyObject = frameworkContentElement.Parent;
            }
            return dependencyObject;
        }
    }
}