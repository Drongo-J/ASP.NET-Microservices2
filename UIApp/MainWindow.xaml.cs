using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UIApp.Entities;

namespace UIApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private Contact contact;

        public Contact Contact
        {
            get { return contact; }
            set { contact = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Contact> allContacts;

        public ObservableCollection<Contact> AllContacts
        {
            get { return allContacts; }
            set { allContacts = value; OnPropertyChanged(); }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            AllContacts = new ObservableCollection<Contact>();
            Contact = new Contact();
        }

        HttpClient httpClient = new HttpClient();

        HttpResponseMessage response = new HttpResponseMessage();


        private async void GetAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetAllContacts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void GetAllContacts()
        {
            response = await httpClient.GetAsync($"https://localhost:22950/c");

            var str = await response.Content.ReadAsStringAsync();

            var items = JsonConvert.DeserializeObject<List<Contact>>(str);

            AllContacts = new ObservableCollection<Contact>(items);
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Contact != null && Contact.Id != 0)
            {
                response = await httpClient.DeleteAsync($"https://localhost:22950/c/{Contact.Id}");

                if (response.StatusCode >= System.Net.HttpStatusCode.NoContent)
                {
                    GetAllContacts();

                    MessageBox.Show("Deleted!");
                }
            }
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            var first_name = firstname.Text;

            var last_name = lastname.Text;

            var contact = new Contact()
            {
                 Firstname = first_name,

                 Lastname = last_name
            };

            var content = JsonConvert.SerializeObject(contact);

            var buffer = Encoding.UTF8.GetBytes(content);

            var byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            response = await httpClient.PostAsync($"https://localhost:22950/c", byteContent);

            var str = await response.Content.ReadAsStringAsync();

            var item = JsonConvert.DeserializeObject<Contact>(str);

            if (item.Id != 0)
            {
                GetAllContacts();
                MessageBox.Show("Added");
            }
        }
    }
}
