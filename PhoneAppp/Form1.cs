using PhoneAppp.Models;// ilk olarak usinglere Models'i ekledik.
using System;
using System.Linq;
using System.Windows.Forms;

namespace PhoneAppp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        PhoneBookEntities db = new PhoneBookEntities();// içerideki db sete ulaşmak için bir instance alıyoruz.


        void Clear()
        {
            foreach (Control item in groupBox1.Controls)
            {
                if (item is TextBox)
                    item.Text = "";
            }
        }

        void KisiListesi() // kişileri listeye ekleyen bir metot oluşturuldu
        {
            lstKisiler.Items.Clear();
            var kisiler = db.People.ToList(); // kişileri bu listeye ekliyoruz.

            foreach (Person person in kisiler) //listeniin içinde ne kadar kişi varsa herbirinde dön ve herbirini listeye ekle.(pluralsize'ı işaretlemiştik a, kişileri tekilleştirdi.)
            {
                ListViewItem lvi = new ListViewItem();// her bir listviewitem satırdır.
                lvi.Text = person.FirstName;  // bunların herbirini yanyana ekler.Hangı sırada yazarsak o sırayla sutunlara ekler.
                lvi.SubItems.Add(person.LastName);
                lvi.SubItems.Add(person.Phone);
                lvi.SubItems.Add(person.Mail);
                lvi.Tag = person.Id;

                lstKisiler.Items.Add(lvi);
            }
        }

        void KisiListesi(string param)// içine string bir parametre alan metot yazıp  adı soyadı vs kısımlaında x değeri içeren sonuçları listeledik.
        {
            lstKisiler.Items.Clear();
            var kisiler = db.People.Where(x =>

             x.FirstName.Contains(param) ||
             x.LastName.Contains(param) ||
             x.Phone.Contains(param) ||
             x.Mail.Contains(param)

            ).ToList();

            foreach (Person person in kisiler)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = person.FirstName;
                lvi.SubItems.Add(person.LastName);
                lvi.SubItems.Add(person.Phone);
                lvi.SubItems.Add(person.Mail);
                lvi.Tag = person.Id;//Tag her kontrolede varsır.Her bir satır yine bir kontrol.Artık arka tarafta her personelin gizli bir şekilde datası da duruyor.

                lstKisiler.Items.Add(lvi);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            KisiListesi();
        }

        private void tsmYeniEkle_Click(object sender, EventArgs e)
        {
            txtAdi.Text = FakeData.NameData.GetFirstName();
            txtSoyadi.Text = FakeData.NameData.GetSurname();
            txtTelefon.Text = FakeData.PhoneNumberData.GetPhoneNumber();
            txtMail.Text = $"{txtAdi.Text}.{txtSoyadi.Text}@{FakeData.NameData.GetCompanyName()}.com".ToLower().Replace(" ", "");//replace boşluğ temizler.
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            Person person = new Person();
            person.FirstName = txtAdi.Text;
            person.LastName = txtSoyadi.Text;
            person.Mail = txtMail.Text;
            person.Phone = txtTelefon.Text;

            db.People.Add(person);// db. people tablosuna person tipinde nesne gönderdik.Excute et eklenen nesneleri gör.
            bool result = db.SaveChanges() > 0; // değişiklikleri onayla.Savechange metodu intiger döner.Sonuc 0'dan buyukse kayıt eklendi
            //savechange metodu eklenen kayıt sayısını verir.

            //int count = db.SaveChanges();
            //if (count > 0)
            //{
            //    result = true;
            //}
            //else
            //{
            //    result = false;
            //}

            
            KisiListesi(); // kaydettiklerimizi görüyouz.
            Clear(); // temizleyip ekliyoruzz ki en baştan tüm eklenenler gözükmesin.

            MessageBox.Show(result ? "Kayıt Eklendi" : "İşlem Hatası"); // result true ise kayıt eklendi değilse işlem hatası;
        }

        private void tsmSil_Click(object sender, EventArgs e)
        {
            if (lstKisiler.SelectedItems.Count > 0)// eleman seçmiş  ise
            {

                DialogResult dr = MessageBox.Show("Kişiyi silmek istiyormusunuz?\nİşlem geri alınamaz!", "Kişi Silme Bildirimi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes) // dialog result bir enumdır.
                {
                    int id = (int)lstKisiler.SelectedItems[0].Tag;
                    Person silinecek = db.People.Find(id);// people'ın içinden id'ye göre bul.
                    db.People.Remove(silinecek);
                    db.SaveChanges();
                    KisiListesi();
                }
                else
                {
                    MessageBox.Show("Kayıt Silme İşlemi İptal Edildi!");
                }
            }
            else//kişi seçilmemişse
            {
                MessageBox.Show("Lütfen bir kişi seçiniz!");
            }
        }



        Person guncellenecek;
        private void tsmDuzenle_Click(object sender, EventArgs e)
        {
            if (lstKisiler.SelectedItems.Count > 0)
            {
                int id = (int)lstKisiler.SelectedItems[0].Tag;
                guncellenecek = db.People.Find(id);  // Find metodu primary key değerine göre size o kişi teslim eder.
                txtAdi.Text = guncellenecek.FirstName;
                txtSoyadi.Text = guncellenecek.LastName;
                txtTelefon.Text = guncellenecek.Phone;
                txtMail.Text = guncellenecek.Mail;
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {

            if (guncellenecek == null)
            {
                MessageBox.Show("Lütfen bir kayıt seçiniz!");
                return; // return ekledik ki Alttaki kodları çalıştırma direk metoodu terket.
            }
            guncellenecek.FirstName = txtAdi.Text;
            guncellenecek.LastName = txtSoyadi.Text;
            guncellenecek.Mail = txtMail.Text;
            guncellenecek.Phone = txtTelefon.Text;

            db.SaveChanges();
            Clear();
            KisiListesi();
            guncellenecek = null;// ?
        }

        private void txtArama_TextChanged(object sender, EventArgs e)
        {
            KisiListesi(txtArama.Text);// arama yapıp listeleyen metodun içerisine txtarama textine girilen değeri verdik
        }
    }
}

