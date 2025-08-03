using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace gelecege_not_proje_odevi
{
    public partial class Form1 : Form
    {
        MySqlConnection connection;
        // MySQL bağlantı dizesi
        private string connectionString = "server=localhost;user=root;database=future;port=3306;password=";
        public Form1()
        {
            InitializeComponent();
            // Veritabanı bağlantısını
            InitalizeDatabase();
            // Notları yüklüme
            notlari_yukle();
            // Bügünkü notları gösterme
            simdiki_not();


        }
        private void InitalizeDatabase()
        {
            // MySQL bağlantısı 
            connection = new MySqlConnection(connectionString);

            try
            {    // Veritabanı bağlantısı başalatılıyor
                connection.Open();
            }
            catch (Exception ex)
            {   // Veritabı bğlantı hatası alınırsa kullnıcaya hata kodunu gösteriyoruz
                MessageBox.Show("Veritabanı bağlantı hatası...\n" + ex.Message);
            }

            // Eğer tablo yoksa tablo oluşturan komut
            string createTableQuery = @"                    
                    CREATE TABLE IF NOT EXISTS Notes (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        message TEXT,
                        display_date DATETIME,                  
                        add_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                    );";
            ;
            // Komut çalıştırılıyor
            MySqlCommand command = new MySqlCommand(createTableQuery, connection);
            command.ExecuteNonQuery();

        }


        private void notlari_yukle()
        {
            //Tablodan verileri seçtiğimiz komut
            string selectQuery = "SELECT id, message, display_date FROM Notes;";

            MySqlCommand command = new MySqlCommand(selectQuery, connection);

            // Tablodaki verileri tablo bitene kadar okuyoruz listboxlara ekliyoruz
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string message = reader.GetString(1);
                    DateTime displayDate = reader.GetDateTime(2);

                    // Geçmiş notlar
                    if (displayDate < DateTime.Today)
                    {
                        listBox2.Items.Add($"[{displayDate}] {message}");
                    }
                    // Gelecek veya bugünkü notlar için
                    else
                    {
                        listBox1.Items.Add($"[{displayDate}] {message}");
                    }
                }
            }
        }

        private void simdiki_not()
        {
            string selectQuery = "SELECT message FROM Notes WHERE DATE(display_date) = CURDATE();";

            MySqlCommand command = new MySqlCommand(selectQuery, connection);

            // Bugünkü notaları bulup hepsini bir message nesnesinde alt alta yazan ve messageboxta gösteren kod
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    string message = "Bugünün notları:\n\n";

                    while (reader.Read())
                    {
                        message += $"{reader.GetString(0)}\n";
                    }

                    MessageBox.Show(message, "Bugünün Notları");
                }
                else
                {
                    MessageBox.Show("Bugün için not bulunamadı.", "Bugünün Notları");
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void button2_Click(object sender, EventArgs e)
        {
            // kendi tasarladığımız close button
            Close();
        }

        private void btnNotEkle_Click(object sender, EventArgs e)
        {
            // textboxtaki, verileri mesaj değişkenine aktarıyoruz
            string mesaj = txtNot.Text;
            // textbox veri girişin bool olarak kaydediyoruz
            bool textboxkontrol = string.IsNullOrEmpty(txtNot.Text);
            // tarihi display_note değişkenine aktarıyoruz
            DateTime display_date = dateTimePicker1.Value;

            // textbox ta veri kontrolü yapıyoruz eğer yoksa not giriniz diye hata mesajı gösteriyoruz.
            if (!textboxkontrol)
            {

                // komut stringi
                string insertquery = @" INSERT INTO Notes (message, display_date) VALUE (@mesaj , @display_date);";

                MySqlCommand command = new MySqlCommand(insertquery, connection);

                // Aldığımız verileri komut satırına ekliyoruz
                command.Parameters.AddWithValue("@mesaj", mesaj);
                command.Parameters.AddWithValue("@display_date", display_date);

                command.ExecuteNonQuery();

                MessageBox.Show("Notunuz Başarıyla kaydedildi !!! \nİyi günler :)");

            }
            else
            {
                MessageBox.Show("Al işte bozdun:( \nLütfen not gir düzelsin ");
            }

            //textbox ve datatimepicker sıfırlanıyor
            txtNot.Text = " ";
            dateTimePicker1.Value = DateTime.Now;

            // listboxları temizleyip yeni eklediğimiz notu ekliyoruz
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            notlari_yukle();
        }
    }
}
