using NotMVVMProject.Data;
using NotMVVMProject.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NotMVVMProject.Views
{
    /// <summary>
    /// Класс ProductsWindow наследуется от BaseWindow, где реализован INotifyPropertyChanged, наслодование в другом частичном классе
    /// </summary>
    public partial class ProductsWindow : INotifyPropertyChanged
    {
        #region Fields
        private int maxDisplayedPages = 5;
        private ObservableCollection<Products> filteredProducts;
        private ObservableCollection<int> pagesNumbers;
        private ObservableCollection<int> displayedPagesNumbers;
        private Products selectedProduct;
        private int selectedPageNumber;
        private int selectedPageIndex;
        private string selectedType;
        private FilterItem selectedSort;
        private int maxItemsOnPage = 3;
        private string searchText;
        private bool orderByDescening;
        #endregion

        #region Properties
        public Products SelectedProduct { get => selectedProduct; set { selectedProduct = value; OnPropertyChanged(); } }
        public ObservableCollection<int> PagesNumbers { get => pagesNumbers; set { pagesNumbers = value; OnPropertyChanged(); } }

        public int SelectedPageNumber { get => selectedPageNumber; set { selectedPageNumber = value; OnPropertyChanged(); } }
        public int SelectedPageIndex { get => selectedPageIndex; set { selectedPageIndex = value; OnPropertyChanged(); } }
        public ObservableCollection<int> DisplayedPagesNumbers { get => displayedPagesNumbers; set { displayedPagesNumbers = value; OnPropertyChanged(); } }
        public bool OrderByDescening { get => orderByDescening; set { orderByDescening = value; OnPropertyChanged(nameof(OrderByDescening)); FilterProducts(SearchText, SelectedType, SelectedSort.Property, orderByDescening); } }
        public string SearchText { get => searchText; set { searchText = value; OnPropertyChanged(nameof(SearchText)); FilterProducts(SearchText, SelectedType, SelectedSort.Property, orderByDescening); RefreshPages(); } }
        private int MaxPage
        {
            get => Convert.ToInt32(Math.Ceiling((float)Products
                    .Where(p => p.ProductName
                    .Contains(SearchText))
                    .Where(p => SelectedType.Equals("Все типы") ? p.Type.Contains("") : p.Type.Equals(SelectedType)).Count() / (float)maxItemsOnPage));
        }
        public string DispayPages { get => $"{SelectedPageNumber}/{MaxPage}"; }
        private ObservableCollection<Products> Products { get; set; }
        public ObservableCollection<Products> FilteredProducts
        {
            get => filteredProducts;
            set
            {
                filteredProducts = value;
                OnPropertyChanged(nameof(FilteredProducts));
            }
        }
        public List<string> FilterTypes { get; set; }
        public List<FilterItem> SortParams { get; set; }
        public string SelectedType { get => selectedType; set { selectedType = value; OnPropertyChanged(nameof(SelectedType)); FilterProducts(SearchText, SelectedType, SelectedSort.Property, orderByDescening); RefreshPages(); } }
        public FilterItem SelectedSort { get => selectedSort; set { selectedSort = value; OnPropertyChanged(nameof(SelectedSort)); FilterProducts(SearchText, SelectedType, SelectedSort.Property, orderByDescening); } }
        #endregion

        public ProductsWindow()
        {
            LoadProducts();
            LoadFilter();
            LoadSortParams();

            InitializeFields();

            InitializeComponent();

            // Устанавливаем костекстом данных этот же класс
            DataContext = this;
        }



        #region Methods
        /// <summary>
        /// Инициализация полей
        /// </summary>
        private void InitializeFields()
        {
            searchText = string.Empty;
            selectedSort = SortParams[0];
            selectedType = FilterTypes[0];
            LoadPages();
            DisplayedPagesNumbers = new ObservableCollection<int>(PagesNumbers.Take(maxDisplayedPages));
            SelectedPageNumber = 1;
        }
        /// <summary>
        /// Заполняем список страниц новыми номерами
        /// </summary>
        private void LoadPages()
        {
            PagesNumbers = new ObservableCollection<int>();
            for (int i = 0; i < MaxPage; i++)
            {
                PagesNumbers.Add(i + 1);
            }
        }
        private void RefreshPages()
        {
            LoadPages();
            DisplayedPagesNumbers = new ObservableCollection<int>(PagesNumbers
                    .Take(maxDisplayedPages));
            if(SelectedPageNumber > DisplayedPagesNumbers.Count)
            {
                SelectedPageNumber = DisplayedPagesNumbers.LastOrDefault();
            }
        }
        /// <summary>
        /// Загрузка продуктов из базы данных
        /// </summary>
        private void LoadProducts()
        {
            using (var db = new TestProductsContext())
            {
                Products = new ObservableCollection<Products>(db.Products.Include("MaterialToProduct").ToList());
            }
        }
        /// <summary>
        /// Добавление параметров соритровки
        /// </summary>
        private void LoadSortParams()
        {
            SortParams = new List<FilterItem>
            {
                new FilterItem("ProductName","Название" ),
                new FilterItem("Amount","Количество")
            };
        }
        /// <summary>
        /// Добавление фильтра
        /// </summary>
        private void LoadFilter()
        {
            FilterTypes = new List<string>()
            {
                    "Все типы",
                    "Подушка",
                    "Стул",
                    "Стол"
            };
        }
        /// <summary>
        /// Фильтрация списка продуктов
        /// </summary>
        /// <param name="search"></param>
        /// <param name="filterType"></param>
        /// <param name="orderBy"></param>
        /// <param name="orderByDescening"></param>
        private void FilterProducts(string search, string filterType, string orderBy = "ProductName", bool orderByDescening = false)
        {
            if (orderByDescening)
            {
                FilteredProducts = new ObservableCollection<Products>(
                Products.OrderByDescending(p => p.GetProperty(orderBy))
                .Where(p => p.ProductName.Contains(search))
                .Where(p => filterType == "Все типы" ? p.Type.Contains("") : p.Type.Equals(filterType))
                .Skip((SelectedPageNumber - 1) * maxItemsOnPage).Take(maxItemsOnPage));
            }
            else
            {
                FilteredProducts = new ObservableCollection<Products>(
                Products.OrderBy(p => p.GetProperty(orderBy))
                .Where(p => p.ProductName.Contains(search))
                .Where(p => filterType == "Все типы" ? p.Type.Contains("") : p.Type.Equals(filterType))
                .Skip((SelectedPageNumber - 1) * maxItemsOnPage).Take(maxItemsOnPage));

            }

            OnPropertyChanged(nameof(DispayPages));
        }

        private void ChangePage()
        {
            //if (SelectedPageIndex >= Math.Ceiling((float)DisplayedPagesNumbers.Count / (float)2))

            if (SelectedPageNumber <= PageListAvg(DisplayedPagesNumbers))
            {
                DisplayedPagesNumbers = new ObservableCollection<int>(PagesNumbers
                    .Take(maxDisplayedPages));
            }
            else
            {
                if (PagesNumbers.Skip(SelectedPageNumber - PageListAvg(DisplayedPagesNumbers)).Count() > maxDisplayedPages)
                    DisplayedPagesNumbers = new ObservableCollection<int>(PagesNumbers
                        .Skip(SelectedPageNumber - PageListAvg(DisplayedPagesNumbers))
                        .Take(maxDisplayedPages));

                else
                    DisplayedPagesNumbers = new ObservableCollection<int>(PagesNumbers
                       .Skip(PagesNumbers.Count - maxDisplayedPages)
                       .Take(maxDisplayedPages));

            }

            FilterProducts(SearchText, SelectedType, SelectedSort.Property, orderByDescening);
        }

        private int PageListAvg(IEnumerable<int> collection)
        {
            return Convert.ToInt32(Math.Ceiling(collection.Count() / (float)2));
        }


        #endregion

        #region UIEventHanlers
        /// <summary>
        /// Выборка сортировки по возрастанию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotDescening_Checked(object sender, RoutedEventArgs e)
        {
            OrderByDescening = false;
        }
        /// <summary>
        /// Выборка сортировки по убыванию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Descening_Checked(object sender, RoutedEventArgs e)
        {
            OrderByDescening = true;
        }
        /// <summary>
        /// Двойной клик по жлементу ListView и переход на форму редактирования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            using (var db = new TestProductsContext())
            {
                var productWindow = new ProductWindow(SelectedProduct);
                productWindow.ShowDialog();
                if (productWindow.DialogResult == true)
                {
                    try
                    {
                        var product = db.Products.Find(productWindow.CurrentProduct.ProductName);
                        product.Type = productWindow.CurrentProduct.Type;
                        product.Supplier = productWindow.CurrentProduct.Supplier;
                        product.ImagePath = productWindow.CurrentProduct.ImagePath;
                        product.Amount = productWindow.CurrentProduct.Amount;

                        product.MaterialToProduct.Clear();
                        product.MaterialToProduct = productWindow.CurrentProduct.MaterialToProduct;

                        db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        LoadProducts();
                        FilterProducts(SearchText, SelectedType, SelectedSort.Property, OrderByDescening);
                        MessageBox.Show("Товар успешно обновлён!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Произошла ошибка при редактировании", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    LoadProducts();
                    FilterProducts(SearchText, SelectedType, SelectedSort.Property, OrderByDescening);
                    MessageBox.Show("Продукт успешно удалён из базы данных!", "Вопрос", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        /// <summary>
        /// Переход на форму добавления продукта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var productWindow = new ProductWindow();
            productWindow.ShowDialog();
            if (productWindow.DialogResult == true)
            {
                using (var db = new TestProductsContext())
                {
                    try
                    {

                        db.Products.Add(productWindow.CurrentProduct);
                        db.SaveChanges();
                        LoadProducts();
                        FilterProducts(SearchText, SelectedType, SelectedSort.Property, OrderByDescening);
                        MessageBox.Show("Товар успешно добавлен!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Произошла ошибка при добавблении", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        #endregion

        #region PropertyChangedEvent
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }







        #endregion



        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangePage();
        }

        private void toFirstPage_Click(object sender, RoutedEventArgs e)
        {
            SelectedPageNumber = 1;
            ChangePage();
        }

        private void toLastPage_Click(object sender, RoutedEventArgs e)
        {
            SelectedPageNumber = PagesNumbers.Count;
            ChangePage();
        }
    }
}
