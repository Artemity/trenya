using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using экзамка.DB;

namespace экзамка
{
    public partial class AddEditWindow : Window
    {
        private LanguagesOfWorldContext _context;

        public AddEditWindow(LanguagesOfWorldContext context)
        {
            InitializeComponent();
            _context = context;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Валидация всех полей
            if (!ValidateFields())
                return;

            try
            {
                // Проверяем существование страны
                string countryName = txtCountryName.Text.Trim();
                var existingCountry = _context.Countries
                    .FirstOrDefault(c => c.CountryName == countryName);

                if (existingCountry == null)
                {
                    // Создаем новую страну
                    existingCountry = new Country
                    {
                        CountryName = countryName,
                        Continent = txtContinent.Text.Trim(),
                        Capital = txtCapital.Text.Trim(),
                        Population = int.Parse(txtPopulation.Text.Trim())
                    };
                    _context.Countries.Add(existingCountry);
                    _context.SaveChanges(); // Сохраняем, чтобы получить CountryCode
                }

                // Проверяем существование языка
                string languageName = txtLanguageName.Text.Trim();
                var existingLanguage = _context.Languages
                    .FirstOrDefault(l => l.LanguageName == languageName);

                if (existingLanguage == null)
                {
                    // Создаем новый язык
                    existingLanguage = new Language
                    {
                        LanguageName = languageName,
                        LanguageGroup = txtLanguageGroup.Text.Trim(),
                        WritingSystem = txtWritingSystem.Text.Trim()
                    };
                    _context.Languages.Add(existingLanguage);
                    _context.SaveChanges(); // Сохраняем, чтобы получить LanguageCode
                }

                // Проверяем, нет ли уже такой связи
                var existingRelation = _context.EthnicCompositions
                    .FirstOrDefault(ec => ec.CountryCode == existingCountry.CountryCode &&
                                         ec.LanguageCode == existingLanguage.LanguageCode);

                if (existingRelation != null)
                {
                    MessageBox.Show($"Связь между страной \"{countryName}\" и языком \"{languageName}\" уже существует!",
                        "Дубликат", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Создаем новую запись в EthnicComposition
                var newComposition = new EthnicComposition
                {
                    CountryCode = existingCountry.CountryCode,
                    LanguageCode = existingLanguage.LanguageCode,
                    SpeakersCount = int.Parse(txtSpeakersCount.Text.Trim())
                };

                _context.EthnicCompositions.Add(newComposition);
                _context.SaveChanges();

                MessageBox.Show($"Запись успешно добавлена!\n\n" +
                              $"Страна: {countryName}\n" +
                              $"Язык: {languageName}\n" +
                              $"Носителей: {txtSpeakersCount.Text} тыс. чел.",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}\n\n{ex.InnerException?.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateFields()
        {
            // Проверка страны
            if (string.IsNullOrWhiteSpace(txtCountryName.Text))
            {
                ShowError(txtCountryName, "Введите название страны!");
                return false;
            }

            // Проверка континента
            if (string.IsNullOrWhiteSpace(txtContinent.Text))
            {
                ShowError(txtContinent, "Введите название континента!");
                return false;
            }

            // Проверка столицы
            if (string.IsNullOrWhiteSpace(txtCapital.Text))
            {
                ShowError(txtCapital, "Введите название столицы!");
                return false;
            }

            // Проверка населения
            if (!int.TryParse(txtPopulation.Text.Trim(), out int population) || population <= 0)
            {
                ShowError(txtPopulation, "Введите корректное население (целое положительное число)!");
                return false;
            }

            // Проверка языка
            if (string.IsNullOrWhiteSpace(txtLanguageName.Text))
            {
                ShowError(txtLanguageName, "Введите название языка!");
                return false;
            }

            // Проверка языковой группы
            if (string.IsNullOrWhiteSpace(txtLanguageGroup.Text))
            {
                ShowError(txtLanguageGroup, "Введите название языковой группы!");
                return false;
            }

            // Проверка письменности
            if (string.IsNullOrWhiteSpace(txtWritingSystem.Text))
            {
                ShowError(txtWritingSystem, "Введите систему письменности!");
                return false;
            }

            // Проверка носителей
            if (!int.TryParse(txtSpeakersCount.Text.Trim(), out int speakers) || speakers <= 0)
            {
                ShowError(txtSpeakersCount, "Введите корректное количество носителей (целое положительное число)!");
                return false;
            }

            return true;
        }

        private void ShowError(TextBox textBox, string message)
        {
            textBox.BorderBrush = new SolidColorBrush(Colors.Red);
            textBox.BorderThickness = new Thickness(2);
            textBox.Focus();
            textBox.SelectAll();

            MessageBox.Show(message, "Ошибка валидации",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}