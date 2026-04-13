using System.Collections.ObjectModel;
using System.ComponentModel;
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
using экзамка.DB;

namespace экзамка
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User _currentUser;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;

            UpdateUserInfo();
            LoadData();
        }

        private void UpdateUserInfo()
        {
            txtUserInfo.Text = _currentUser.Login;
            txtRoleInfo.Text = GetRoleDisplayName(_currentUser.RoleName);
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
                // Проверяем роль - если гость, то не загружаем данные
                if (_currentUser.RoleName == "Guest")
                {
                    lvData.ItemsSource = null;
                    txtStatus.Text = "Гостевой режим - данные не отображаются";
                    txtRecordCount.Text = "Доступ ограничен";
                    return;
                }

                // Для Admin и Manager загружаем данные
                using (var context = new LanguagesOfWorldContext())
                {
                    lvData.ItemsSource = context.Countries.ToList();
                    txtStatus.Text = "Данные загружены";
                    txtRecordCount.Text = $"Всего записей: {context.Countries.Count()}";
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "Ошибка загрузки данных";
                txtRecordCount.Text = "";
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}