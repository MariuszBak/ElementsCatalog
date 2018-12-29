using System;
using System.Collections.Generic;
using System.ComponentModel;
using ECapp.Models.Entities;
using System.Windows.Input;
using ECapp.Models;

namespace ECapp.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        private string searchPhrase;
        private List<ElementShort> elements = new List<ElementShort>();
        private ElementShort selectedElement;
        private Element editedElement = new Element();
        private bool editedElementIsSaved;
        private bool editedElementIsChanged;
        private DbRepos dbRepos;

        public ICommand NewElementCommand { get; }
        public ICommand CopyToNewElementCommand { get; }
        public ICommand DeleteElementCommand { get; }
        public ICommand SaveElementCommand { get; }
        public ICommand QuantityIncCommand { get; }
        public ICommand QuantityDecCommand { get; }

        public MainWindowViewModel() { }

        public MainWindowViewModel(DbRepos dbRepos)
        {
            this.dbRepos = dbRepos;

            NewElementCommand = new RelayCommand(p => NewElementAction());
            CopyToNewElementCommand = new RelayCommand(p => CopyToNewElementAction());
            DeleteElementCommand = new RelayCommand(p => DeleteElementAction());
            SaveElementCommand = new RelayCommand(p => SaveElementAction());
            QuantityIncCommand = new RelayCommand(p => QuantityIncAction());
            QuantityDecCommand = new RelayCommand(p => QuantityDecAction());

            elements = dbRepos.GetAllElementsShort();
            NewElementAction();
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(params string[] propertyNames)
        {
            if (PropertyChanged != null)
            {
                foreach (string propertyName in propertyNames)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
        #endregion


        public bool EditedElementIsChanged
        {
            get { return editedElementIsChanged; }
        }

        public string SearchPhrase
        {
            get
            {
                return searchPhrase;
            }
            set
            {
                searchPhrase = value;

                Console.WriteLine("Search: " + searchPhrase);
                elements = dbRepos.GetElementsShort(searchPhrase);
                Console.WriteLine("Found " + elements.Count + " element(s).");

                OnPropertyChanged("SearchPhrase");
                OnPropertyChanged("Elements");
            }
        }

        public List<ElementShort> Elements
        {
            get
            {
                return elements;
            }
            set
            {
                elements = value;
                OnPropertyChanged("Elements");
            }
        }

        public ElementShort SelectedElement
        {
            get
            {
                return selectedElement;
            }
            set
            {
                if (value != null)
                {
                    selectedElement = value;
                    OnPropertyChanged("SelectedElement");

                    editedElement = dbRepos.GetElement(selectedElement.Id);
                    OnPropertyChanged("EditorName");
                    OnPropertyChanged("EditorDesc");
                    OnPropertyChanged("EditorPackage");
                    OnPropertyChanged("EditorCategory");
                    OnPropertyChanged("EditorStatus");
                    OnPropertyChanged("EditorContainer");
                    OnPropertyChanged("EditorQuantity");

                    editedElementIsChanged = false;
                    editedElementIsSaved = true;
                    OnPropertyChanged("EditorInfo");
                    OnPropertyChanged("ButtonSaveIsEnabled");
                    Console.WriteLine("Select " + selectedElement.Id);

                }
                else
                    selectedElement = new ElementShort();
            }
        }

        public string EditorInfo
        {
            get
            {
                if (editedElement.Id > 0)
                    if (editedElementIsSaved)
                        return "Element from the database";
                    else
                        return "Element is unsaved";

                else
                    return "New element";
            }
        }

        public string EditorName
        {
            get
            {
                return editedElement.Name;
            }
            set
            {
                editedElementIsChanged = true;
                editedElementIsSaved = false;
                OnPropertyChanged("ButtonSaveIsEnabled");
                editedElement.Name = value;
                OnPropertyChanged("EditorName");
                OnPropertyChanged("EditorInfo");
            }
        }

        public string EditorDesc
        {
            get
            {
                return editedElement.Desc;
            }
            set
            {
                editedElementIsChanged = true;
                editedElementIsSaved = false;
                OnPropertyChanged("ButtonSaveIsEnabled");
                editedElement.Desc = value;
                OnPropertyChanged("EditorDesc");
                OnPropertyChanged("EditorInfo");
            }
        }

        public string EditorPackage
        {
            get
            {
                return editedElement.Package;
            }
            set
            {
                editedElementIsChanged = true;
                editedElementIsSaved = false;
                OnPropertyChanged("ButtonSaveIsEnabled");
                editedElement.Package = value;
                OnPropertyChanged("EditorPackage");
                OnPropertyChanged("EditorInfo");
            }
        }

        public string EditorCategory
        {
            get
            {
                return editedElement.Category;
            }
            set
            {
                editedElementIsChanged = true;
                editedElementIsSaved = false;
                OnPropertyChanged("ButtonSaveIsEnabled");
                editedElement.Category = value;
                OnPropertyChanged("EditorCategory");
                OnPropertyChanged("EditorInfo");
            }
        }

        public string EditorStatus
        {
            get
            {
                return editedElement.Status;
            }
            set
            {
                editedElementIsChanged = true;
                editedElementIsSaved = false;
                OnPropertyChanged("ButtonSaveIsEnabled");
                editedElement.Status = value;
                OnPropertyChanged("EditorStatus");
                OnPropertyChanged("EditorInfo");
            }
        }

        public string EditorContainer
        {
            get
            {
                return editedElement.Container;
            }
            set
            {
                editedElementIsChanged = true;
                editedElementIsSaved = false;
                OnPropertyChanged("ButtonSaveIsEnabled");
                editedElement.Container = value;
                OnPropertyChanged("EditorContainer");
                OnPropertyChanged("EditorInfo");
            }
        }

        public string EditorQuantity
        {
            get
            {
                return editedElement.Quantity.ToString();
            }
            set
            {
                editedElementIsChanged = true;
                editedElementIsSaved = false;
                OnPropertyChanged("ButtonSaveIsEnabled");
                long.TryParse(value, out long quantity);
                editedElement.Quantity = quantity;
                OnPropertyChanged("EditorQuantity");
                OnPropertyChanged("EditorInfo");
            }
        }

        public Element EditedElement
        {
            get
            {
                return editedElement;
            }
            set
            {
                editedElement = value;
                if (editedElement.Id > 0)
                {
                    editedElementIsChanged = false;
                    editedElementIsSaved = true;
                }
                else
                {
                    if (!EditorIsEmpty())
                    {
                        editedElementIsChanged = true;
                        editedElementIsSaved = false;
                    }
                }

                OnPropertyChanged("EditorName");
                OnPropertyChanged("EditorDesc");
                OnPropertyChanged("EditorPackage");
                OnPropertyChanged("EditorCategory");
                OnPropertyChanged("EditorStatus");
                OnPropertyChanged("EditorContainer");
                OnPropertyChanged("EditorQuantity");

                OnPropertyChanged("EditorInfo");
                OnPropertyChanged("ButtonSaveIsEnabled");
            }
        }

        public bool ButtonSaveIsEnabled
        {
            get
            {
                return editedElementIsChanged & !editedElementIsSaved;
            }
        }

        private void NewElementAction()
        {
            editedElement = new Element();

            OnPropertyChanged("EditorName");
            OnPropertyChanged("EditorDesc");
            OnPropertyChanged("EditorPackage");
            OnPropertyChanged("EditorCategory");
            OnPropertyChanged("EditorStatus");
            OnPropertyChanged("EditorContainer");
            OnPropertyChanged("EditorQuantity");

            editedElementIsChanged = false;
            editedElementIsSaved = true;
            OnPropertyChanged("EditorInfo");
            OnPropertyChanged("ButtonSaveIsEnabled");

            Console.WriteLine("New element");
        }

        private void CopyToNewElementAction()
        {
            editedElement.Id = 0;

            editedElementIsChanged = true;
            editedElementIsSaved = false;
            OnPropertyChanged("EditorInfo");
            OnPropertyChanged("ButtonSaveIsEnabled");

            Console.WriteLine("Copy to new element");
        }

        private void DeleteElementAction()
        {
            dbRepos.DeleteElement(editedElement.Id);

            elements = dbRepos.GetElementsShort(searchPhrase);
            OnPropertyChanged("Elements");

            editedElement = new Element();
            OnPropertyChanged("EditorName");
            OnPropertyChanged("EditorDesc");
            OnPropertyChanged("EditorPackage");
            OnPropertyChanged("EditorCategory");
            OnPropertyChanged("EditorStatus");
            OnPropertyChanged("EditorContainer");
            OnPropertyChanged("EditorQuantity");

            editedElementIsChanged = false;
            editedElementIsSaved = true;
            OnPropertyChanged("EditorInfo");
            OnPropertyChanged("ButtonSaveIsEnabled");

            Console.WriteLine("Delete element " + editedElement.Id);
        }

        private void SaveElementAction()
        {
            if (editedElement.Id > 0)
                dbRepos.UpdateElement(editedElement);
            else
            {
                long id = dbRepos.InsertElement(editedElement);
                editedElement = dbRepos.GetElement(id);
            }

            editedElementIsChanged = false;
            editedElementIsSaved = true;
            OnPropertyChanged("EditorInfo");

            elements = dbRepos.GetElementsShort(searchPhrase);
            OnPropertyChanged("Elements");

            Console.WriteLine("Save element " + editedElement.Id);
        }

        private void QuantityIncAction()
        {
            editedElement.Quantity++;
            editedElementIsChanged = true;
            editedElementIsSaved = false;
            OnPropertyChanged("ButtonSaveIsEnabled");
            OnPropertyChanged("EditorQuantity");
            OnPropertyChanged("EditorInfo");
            Console.WriteLine("Quantity increment");
        }
        private void QuantityDecAction()
        {
            if (editedElement.Quantity > 0)
            {
                editedElement.Quantity--;
                editedElementIsChanged = true;
                editedElementIsSaved = false;
                OnPropertyChanged("ButtonSaveIsEnabled");
                OnPropertyChanged("EditorQuantity");
                OnPropertyChanged("EditorInfo");
                Console.WriteLine("Quantity decrement");
            }
        }

        private bool EditorIsEmpty()
        {
            bool result = true;
            if (editedElement.Name != null)
                if (editedElement.Name.Length > 0) result = true;
            if (editedElement.Desc != null)
                if (editedElement.Desc.Length > 0) result = true;
            if (editedElement.Package != null)
                if (editedElement.Package.Length > 0) result = true;
            if (editedElement.Category != null)
                if (editedElement.Category.Length > 0) result = true;
            if (editedElement.Status != null)
                if (editedElement.Status.Length > 0) result = true;
            if (editedElement.Container != null)
                if (editedElement.Container.Length > 0) result = true;

            return result;
        }
    }

    public class SearchPhraseChangedEventArgs : EventArgs
    {
        public string SearchPhrase { get; set; }
    }

}
