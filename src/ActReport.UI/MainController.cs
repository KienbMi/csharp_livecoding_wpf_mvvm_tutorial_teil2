using ActReport.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace ActReport.UI
{
    public class MainController : IController
    {
        public void ShowWindow(BaseViewModel viewModel)
        {
            Window window = viewModel switch
            {
                // Wenn viewModel null ist -> ArgumentNullException
                null => throw new ArgumentNullException(nameof(viewModel)),

                // Wenn viewModel vom Typ EmployeeViewModel ist -> neues MainWindow instanzieren
                EmployeeViewModel _ => new MainWindow(),
                ActivityViewModel _ => new ActivityWindow(),
                ActivityDetailViewModel _ => new ActivityDetailWindow(),

                // default -> InvalidOperationException
                _ => throw new InvalidOperationException($"Unknown ViewModel of type '{viewModel}'")
            };

            window.DataContext = viewModel;
            window.ShowDialog();
        }
    }
}
