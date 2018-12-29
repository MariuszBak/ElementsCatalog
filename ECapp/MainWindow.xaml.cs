using ECapp.Models;
using ECapp.Models.Entities;
using ECapp.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ECapp
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DbRepos dbRepos = new DbRepos();
        private ICommand TryLoadElement = new RelayCommand(OnTryLoadElement);
        private MainWindowViewModel viewModel;

        private static void OnTryLoadElement(object obj)
        {
            throw new NotImplementedException();
        }

        public MainWindow()
        {
            InitializeComponent();
            viewModel = DataContext as MainWindowViewModel;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                viewModel.SearchPhrase = "";
                SearchPhrase.Focus();
            }
            base.OnKeyDown(e);
        }

        private void Elements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ElementsSelected();
        }

        private void Elements_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ElementsSelected();
        }

        private void ElementsSelected()
        {
            if (viewModel.EditedElementIsChanged)
            {
                questionDialogBox.Show.Execute("The edited item has not been saved. Do you want to continue?");
                if (questionDialogBox.LastResult == MessageBoxResult.Yes)
                    viewModel.SelectedElement = ElementsList.SelectedItem as ElementShort;
            }
            else
            {
                if (ElementsList.SelectedItem as ElementShort != null && (ElementsList.SelectedItem as ElementShort).Id != viewModel.EditedElement.Id)
                    viewModel.SelectedElement = ElementsList.SelectedItem as ElementShort;
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            questionDialogBox.Show.Execute("Do you really want to delete an element?");
            if (questionDialogBox.LastResult == MessageBoxResult.Yes)
                viewModel.DeleteElementCommand.Execute(null);
        }

        private void CopyToNew_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.EditedElementIsChanged)
            {
                questionDialogBox.Show.Execute("The edited item has not been saved. Do you want to continue?");
                if (questionDialogBox.LastResult == MessageBoxResult.Yes)
                    viewModel.CopyToNewElementCommand.Execute(null);
            }
            else
            {
                viewModel.CopyToNewElementCommand.Execute(null);
            }
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.EditedElementIsChanged)
            {
                questionDialogBox.Show.Execute("The edited item has not been saved. Do you want to continue?");
                if (questionDialogBox.LastResult == MessageBoxResult.Yes)
                    viewModel.NewElementCommand.Execute(null);
            }
            else
            {
                viewModel.NewElementCommand.Execute(null);
            }
        }


    }

    public class SimpleMessageDialogBox : DialogBox
    {
        public SimpleMessageDialogBox()
        {
            execute =
            o =>
            {
                MessageBox.Show((string)o, Caption);
            };
        }
    }

    public class NotificationDialogBox : CommandDialogBox
    {
        public NotificationDialogBox()
        {
            execute =
            o =>
            {
                MessageBox.Show((string)o, Caption, MessageBoxButton.OK, MessageBoxImage.Information);
            };
        }
    }

    public class MessageDialogBox : CommandDialogBox
    {
        public MessageBoxResult? LastResult { get; protected set; }
        public MessageBoxButton Buttons { get; set; } = MessageBoxButton.OK;
        public MessageBoxImage Icon { get; set; } = MessageBoxImage.None;
        public bool IsLastResultYes
        {
            get
            {
                if (!LastResult.HasValue) return false;
                return LastResult.Value == MessageBoxResult.Yes;
            }
        }
        public bool IsLastResultNo
        {
            get
            {
                if (!LastResult.HasValue) return false;
                return LastResult.Value == MessageBoxResult.No;
            }
        }
        public bool IsLastResultCancel
        {
            get
            {
                if (!LastResult.HasValue) return false;
                return LastResult.Value == MessageBoxResult.Cancel;
            }
        }
        public bool IsLastResultOK
        {
            get
            {
                if (!LastResult.HasValue) return false;
                return LastResult.Value == MessageBoxResult.OK;
            }
        }
        public MessageDialogBox()
        {
            execute = o =>
            {
                LastResult = MessageBox.Show((string)o + "\n" + CommandParameter, Caption, Buttons, Icon);
                OnPropertyChanged("LastResult");
                switch (LastResult)
                {
                    case MessageBoxResult.Yes:
                        OnPropertyChanged("IsLastResultYes");
                        ExecuteCommand(CommandYes, CommandParameter);
                        break;
                    case MessageBoxResult.No:
                        OnPropertyChanged("IsLastResultNo");
                        ExecuteCommand(CommandNo, CommandParameter);
                        break;
                    case MessageBoxResult.Cancel:
                        OnPropertyChanged("IsLastResultCancel");
                        ExecuteCommand(CommandCancel, CommandParameter);
                        break;
                    case MessageBoxResult.OK:
                        OnPropertyChanged("IsLastResultOK");
                        ExecuteCommand(CommandOK, CommandParameter);
                        break;
                }
            };
        }
        public static DependencyProperty CommandYesProperty = DependencyProperty.Register("CommandYes", typeof(ICommand), typeof(MessageDialogBox));
        public static DependencyProperty CommandNoProperty = DependencyProperty.Register("CommandNo", typeof(ICommand), typeof(MessageDialogBox));
        public static DependencyProperty CommandCancelProperty = DependencyProperty.Register("CommandCancel", typeof(ICommand), typeof(MessageDialogBox));
        public static DependencyProperty CommandOKProperty = DependencyProperty.Register("CommandOK", typeof(ICommand), typeof(MessageDialogBox));
        public ICommand CommandYes
        {
            get
            {
                return (ICommand)GetValue(CommandYesProperty);
            }
            set
            {
                SetValue(CommandYesProperty, value);
            }
        }
        public ICommand CommandNo
        {
            get
            {
                return (ICommand)GetValue(CommandNoProperty);
            }
            set
            {
                SetValue(CommandNoProperty, value);
            }
        }
        public ICommand CommandCancel
        {
            get
            {
                return (ICommand)GetValue(CommandCancelProperty);
            }
            set
            {
                SetValue(CommandCancelProperty, value);
            }
        }
        public ICommand CommandOK
        {
            get
            {
                return (ICommand)GetValue(CommandOKProperty);
            }

            set
            {
                SetValue(CommandOKProperty, value);
            }
        }
    }

}
