using Microsoft.Win32;
using NotMVVMProject.Data;
using NotMVVMProject.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
    /// Логика взаимодействия для ProductWindow.xaml
    /// Класс реализует интерфейс INotifyPropertyChanged
    /// </summary>
    public partial class ProductWindow : Window, INotifyPropertyChanged
    {
        #region Fields
        //флаг опреедляющие операцияю, добавляется продукт или редактируется
        private bool isOperationAdd;
        private string productName;
        private string selectedType;
        private string supplier;
        private string amount;
        private string search;
        private string image;
        private string tempImage;
        private string amountOfMaterial;
        //материалы в comboBox
        private ObservableCollection<Materials> materials;
        //материалы в lsitView
        private ObservableCollection<ProductMaterialListViewModel> materialsList;
        //выбранный материал в combobBox
        private Materials selectedAddMaterial;
        //выбранный материал в lsitView
        private ProductMaterialListViewModel selectedMaterial;
        #endregion

        #region Properties
        public string ProductName { get => productName; set { productName = value; OnPropertyChanged(); } }
        public Products CurrentProduct { get; set; }
        public List<string> Types { get; set; }
        public string SelectedType { get => selectedType; set { selectedType = value; OnPropertyChanged(); } }
        public string Supplier { get => supplier; set { supplier = value; OnPropertyChanged(); } }
        public string Amount { get => amount; set { amount = value; OnPropertyChanged(); } }
        public string Search { get => search; set { search = value; OnPropertyChanged(); OnPropertyChanged(nameof(FilterMaterials)); } }
        public string AmountOfMaterial { get => amountOfMaterial; set { amountOfMaterial = value; OnPropertyChanged(); } }
        public string Image { get => image; set { image = value; OnPropertyChanged(); } }
        //материалы в comboBox
        public ObservableCollection<Materials> Materials { get => materials; set { materials = value; OnPropertyChanged(); } }
        //это будет есть надо сделать фильтрацию для списка материалов, например
        public ObservableCollection<Materials> FilterMaterials { get => new ObservableCollection<Materials>(Materials.Where(p => p.MaterialName.Contains(Search))); }
        //материалы в lsitView
        public ObservableCollection<ProductMaterialListViewModel> MaterialsList { get => materialsList; set { materialsList = value; OnPropertyChanged(); } }
        //выбранный материал в combobBox
        public Materials SelectedAddMaterial { get => selectedAddMaterial; set { selectedAddMaterial = value; OnPropertyChanged(); } }
        //выбранный материал в lsitView
        public ProductMaterialListViewModel SelectedMaterial { get => selectedMaterial; set { selectedMaterial = value; OnPropertyChanged(); } }

        #endregion

        public ProductWindow()
        {
            isOperationAdd = true;
            DataContext = this;
            MaterialsList = new ObservableCollection<ProductMaterialListViewModel>();
            InitializeStandartFields();
            LoadMaterials();
            InitializeComponent();

        }
        public ProductWindow(Products product)
        {
            isOperationAdd = false;
            DataContext = this;
            MaterialsList = new ObservableCollection<ProductMaterialListViewModel>();

            InitializeFields(product);
            LoadMaterials();
            LoadMaterialsList();

            InitializeComponent();

            ProductNameTextBox.IsReadOnly = true;

            Delete.Visibility = Visibility.Visible;
        }

        #region Methods
        private void InitializeFields(Products product)
        {
            Search = string.Empty;
            CurrentProduct = product;
            ProductName = CurrentProduct.ProductName;
            //выбираем из типов первый элемент, который равен CurrentProduct.Type
            Types = new List<string>
            {
                "Стул",
                "Стол",
                "Подушка"
            };
            SelectedType = Types.FirstOrDefault(p => p.Equals(CurrentProduct.Type));
            Supplier = CurrentProduct.Supplier;
            Image = CurrentProduct.ImagePath;
            Amount = CurrentProduct.Amount.ToString();
        }

        private void InitializeStandartFields()
        {
            CurrentProduct = new Products();
            Search = string.Empty;
            ProductName = string.Empty;
            //выбираем из типов первый элемент, который равен CurrentProduct.Type
            Types = new List<string>
            {
                "Стул",
                "Стол",
                "Подушка"
            };
            SelectedType = string.Empty;
            Supplier = string.Empty;
            Image = string.Empty;
            Amount = string.Empty;
        }
        private void LoadMaterials()
        {
            using (var db = new TestProductsContext())
            {
                Materials = new ObservableCollection<Materials>(db.Materials.ToList());
            }
        }
        private void LoadMaterialsList()
        {
            if (CurrentProduct.MaterialToProduct.ToList().Count > 0)
            {
                foreach (var item in CurrentProduct.MaterialToProduct)
                {
                    MaterialsList.Add(new ProductMaterialListViewModel { MaterialName = item.MaterialName, Amount = item.AmountOfMaterial.ToString() });
                }
            }
        }
        private bool ValidateProduct(Products product)
        {
            if (!string.IsNullOrEmpty(product.Supplier)
                && (!string.IsNullOrEmpty(product.ImagePath))
                && (product.Amount > 0)
                && (!string.IsNullOrEmpty(product.ProductName))
                && (!string.IsNullOrEmpty(product.Type)))
                return true;
            return false;
        }
        private bool ValidateProductMaterial(Materials material)
        {
            if (int.TryParse(AmountOfMaterial, out int amount))
            {
                return true;
            }
            return false;
        }

        #endregion

        #region UIEvendHandlers

        /// <summary>
        /// Загрузка картинки в Image
        /// Здесь идёт копирование нового изображения в папку с картинками с автоматически генерируемым именем и затем Binding к Image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var newFilePath = "Images/" + Guid.NewGuid() + ".png";
                
                tempImage = newFilePath;
                Image = openFileDialog.FileName;
            }
        }

        /// <summary>
        /// Добавление материала из ComboBox в список ListView продукта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddToList_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAddMaterial != null)
            {
                if (ValidateProductMaterial(SelectedAddMaterial))
                {
                    if (MaterialsList.FirstOrDefault(p => p.MaterialName.Equals(SelectedAddMaterial.MaterialName)) is null)
                        MaterialsList.Add(new ProductMaterialListViewModel { MaterialName = SelectedAddMaterial.MaterialName, Amount = AmountOfMaterial });
                    else MessageBox.Show("Этот материал уже есть в списке", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
                else MessageBox.Show("Один или несколько полей введены неверно!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else MessageBox.Show("Сначала выберите материал!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);

        }

        /// <summary>
        /// Удаление материала из ListView продукта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveFromList_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMaterial != null)
            {
                MaterialsList.Remove(MaterialsList.FirstOrDefault(p => p.MaterialName.Equals(SelectedMaterial.MaterialName)));
            }
            else MessageBox.Show("Сначала выберите материал!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        /// <summary>
        /// Добавление нового или изменение существующего продукта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new TestProductsContext())
            {
                CurrentProduct.ImagePath = Image;
                CurrentProduct.Supplier = Supplier;
                CurrentProduct.Type = SelectedType;
                CurrentProduct.Amount = Convert.ToInt32(Amount);
                CurrentProduct.ProductName = ProductName;
                if (ValidateProduct(CurrentProduct))
                {
                    File.Copy(Image, tempImage);
                    //добавление продукта, если флаг равен true, и редактирование продукта, если флаг false
                    if (isOperationAdd)
                    {
                        try
                        {
                            //если продукт с таких именем в базе не найден, то добавляем, иначе коворим что ошибка
                            if (db.Products.FirstOrDefault(p => p.ProductName.Equals(ProductName)) == null)
                            {
                                foreach (var item in MaterialsList)
                                {
                                    CurrentProduct.MaterialToProduct.Add(new MaterialToProduct
                                    {
                                        ProductName = CurrentProduct.ProductName,
                                        MaterialName = item.MaterialName,
                                        AmountOfMaterial = Convert.ToInt32(AmountOfMaterial)
                                    });
                                }
                                this.DialogResult = true;
                            }
                            else
                            {
                                MessageBox.Show($"Продукт с именем {CurrentProduct.ProductName} уже сщуествует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Произошла ошибка при добавлении", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        try
                        {
                            CurrentProduct.MaterialToProduct.Clear();
                            foreach (var item in MaterialsList)
                            {
                                CurrentProduct.MaterialToProduct.Add(new MaterialToProduct
                                {
                                    ProductName = CurrentProduct.ProductName,
                                    MaterialName = item.MaterialName,
                                    AmountOfMaterial = Convert.ToInt32(item.Amount)
                                });
                            }
                            this.DialogResult = true;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Произошла ошибка при редактировании", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else MessageBox.Show("Не все полня правильно заполнены!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Удаление продукта из базы данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new TestProductsContext())
            {
                var dialog = MessageBox.Show("Вы точно хотите удалить этот продукт?", "Опопвещение", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (dialog == MessageBoxResult.OK)
                {
                    var product = db.Products.Find(CurrentProduct.ProductName);
                    if (product != null)
                    {
                        db.Products.Remove(product);
                        db.SaveChanges();
                        DialogResult = false;
                        
                    }
                    else MessageBox.Show("Продукт не найден в базе данных!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Кнопка отмены и возвращения в предыдущее окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        #endregion

        #region PropertyChangedEvent
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion     
    }
}
