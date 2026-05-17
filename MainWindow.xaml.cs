using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using экзамка.DB;

namespace экзамка
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User _currentUser;
        private ObservableCollection<CountryLanguageView> _allData;
        private LanguagesOfWorldContext _context;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            _context = new LanguagesOfWorldContext();

            UpdateUserInfo();
            LoadData();
        }

        private void UpdateUserInfo()
        {
            txtUserInfo.Text = _currentUser.Login;
            txtRoleInfo.Text = GetRoleDisplayName(_currentUser.RoleName);

            // Настройка доступности кнопок в зависимости от роли
            if (_currentUser.RoleName == "Guest")
            {
                // Гости не видят кнопки управления
            }
            else if (_currentUser.RoleName == "Manager")
            {
                // Менеджеры могут только просматривать
            }
        }

        private string GetRoleDisplayName(string role)
        {
            return role switch
            {
                "Admin" => "Администратор",
                "Manager" => "Менеджер",
                "Guest" => "Гость",
                _ => role
            };
        }

        private void LoadData()
        {
            try
            {
                if (_currentUser.RoleName == "Guest")
                {
                    lvData.ItemsSource = null;
                    txtStatus.Text = "Гостевой режим - данные не отображаются";
                    txtRecordCount.Text = "Доступ ограничен";
                    return;
                }

                // Загружаем связанные данные с помощью JOIN
                var query = from country in _context.Countries
                            join ethnic in _context.EthnicCompositions
                                on country.CountryCode equals ethnic.CountryCode
                            join language in _context.Languages
                                on ethnic.LanguageCode equals language.LanguageCode
                            select new CountryLanguageView
                            {
                                CountryCode = country.CountryCode,
                                CountryName = country.CountryName,
                                Continent = country.Continent,
                                Capital = country.Capital,
                                Population = country.Population,
                                LanguageName = language.LanguageName,
                                LanguageGroup = language.LanguageGroup,
                                WritingSystem = language.WritingSystem,
                                SpeakersCount = ethnic.SpeakersCount,
                                LanguageCode = language.LanguageCode,
                                CompositionID = 0
                            };

                _allData = new ObservableCollection<CountryLanguageView>(query.ToList());
                lvData.ItemsSource = _allData;

                txtStatus.Text = "Данные загружены";
                txtRecordCount.Text = $"Всего записей: {_allData.Count}";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "Ошибка загрузки данных";
                txtRecordCount.Text = "";
            }
        }

        // Поиск по языку при изменении текста
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.ToLower().Trim();

            if (_allData == null) return;

            if (string.IsNullOrEmpty(searchText))
            {
                lvData.ItemsSource = _allData;
                txtRecordCount.Text = $"Всего записей: {_allData.Count}";
                txtStatus.Text = "Отображены все записи";
            }
            else
            {
                var filteredData = _allData
                    .Where(x => x.LanguageName.ToLower().Contains(searchText))
                    .ToList();

                lvData.ItemsSource = filteredData;
                txtRecordCount.Text = $"Найдено записей: {filteredData.Count}";
                txtStatus.Text = $"Поиск: \"{txtSearch.Text}\"";
            }
        }

        // Добавление новой записи
        // Добавление новой записи
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.RoleName == "Guest" || _currentUser.RoleName == "Manager")
            {
                MessageBox.Show("Недостаточно прав для добавления записей!", "Ошибка доступа",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AddEditWindow addWindow = new AddEditWindow(_context);
            if (addWindow.ShowDialog() == true)
            {
                LoadData(); // Обновляем данные после добавления
                txtStatus.Text = "Запись успешно добавлена";
            }
        }

        // Удаление выбранной записи
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.RoleName == "Guest" || _currentUser.RoleName == "Manager")
            {
                MessageBox.Show("Недостаточно прав для удаления записей!", "Ошибка доступа",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedItem = lvData.SelectedItem as CountryLanguageView;

            if (selectedItem == null)
            {
                MessageBox.Show("Выберите запись для удаления!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Вы уверены, что хотите удалить запись?\n\n" +
                $"Страна: {selectedItem.CountryName}\n" +
                $"Язык: {selectedItem.LanguageName}\n" +
                $"Носителей: {selectedItem.SpeakersCount} тыс. чел.",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Удаляем по составному ключу (CountryCode + LanguageCode)
                    var ethnicToDelete = _context.EthnicCompositions
                        .FirstOrDefault(x => x.CountryCode == selectedItem.CountryCode &&
                                            x.LanguageCode == selectedItem.LanguageCode);

                    if (ethnicToDelete != null)
                    {
                        _context.EthnicCompositions.Remove(ethnicToDelete);
                        _context.SaveChanges();
                        LoadData();
                        txtStatus.Text = "Запись успешно удалена";
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Обновление данных
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            txtSearch.Clear();
            txtStatus.Text = "Данные обновлены";
        }

        // Класс для отображения связанных данных
        public class CountryLanguageView
        {
            public int CountryCode { get; set; }
            public string CountryName { get; set; }
            public string Continent { get; set; }
            public string Capital { get; set; }
            public int Population { get; set; }
            public string LanguageName { get; set; }
            public string LanguageGroup { get; set; }
            public string WritingSystem { get; set; }
            public int SpeakersCount { get; set; }
            public int LanguageCode { get; set; }
            public int CompositionID { get; set; }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}