using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Mail;

//Uppgiften handlar om att skapa en adressbok.All information skall sparas i en textfil.Det
//betyder att applikationen skall kunna läsa och skriva från denna textfil.
//Krav för att uppgiften skall bli godkänd:
//- Följande information skall kunna registreras och sparas till textfilen, namn, gatudress,
//postnummer, postort, telefon och epost
//- Det skall gå att spara en ny adress i adressboken men också uppdatera en adress som
//redan finns.
//- Det skall gå att ta bort en adress och den skall då försvinna från adressboken.
//- Det skall gå att söka på en eller flera adresser och sökresultatet skall visas i en lista.
//Från listan skall det gå att klicka på en rad och få upp all information om den adressen.
//- Sökningen skall fungera som ett urval där det minst skall gå att göra urval på namn
//och postort.
//Koden skall fungera och applikationen skall gå att köra utan fel.

//ev. gör objektorienterat

namespace Adressbok
{
    public partial class frmAddressBook : Form
    {
        string path = "C:\\Users\\AnnaDaniel\\Documents\\Nackademin\\Adressbok.txt";
        List<string> lines = new List<string>();

        public frmAddressBook()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the ContactInfo.
        /// </summary>
        /// <returns></returns>
        public string ContactInfo()
        {
            string name = txtNamn.Text;
            string street = txtGatuadress.Text;
            string zipCode = txtPostnr.Text;
            string city = txtPostadress.Text;
            string phoneNr = txtTelefonnr.Text;
            string email = txtEmail.Text;
            return name+ ", "+ street+", "+zipCode+", "+city+", "+phoneNr+", "+email;
        }

        /// <summary>
        /// Empties all textboxes in groupbox.
        /// </summary>
        public void ClearTextboxes()
        {
            txtNamn.Clear();
            txtGatuadress.Clear();
            txtPostadress.Clear();
            txtPostnr.Clear();
            txtTelefonnr.Clear();
            txtEmail.Clear();
        }

        /// <summary>
        /// Validates email address.
        /// </summary>
        /// <param name="emailadress"></param>
        /// <returns></returns>
        private bool EmailIsValid(string emailadress)
        {
            int emailLength = emailadress.Length;

            if (emailadress.Contains("@") && !emailadress.Contains(" "))
            {
                if ((emailadress.LastIndexOf('.') == (emailLength - 4)) ||
                    (emailadress.LastIndexOf('.') == (emailLength - 3)))
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Adds what info is missing to string.
        /// </summary>
        /// <returns></returns>
        private string MissingInfoToString()
        {
            string message = "";
            if (string.IsNullOrEmpty(txtNamn.Text))
            {
                message += "Namn\n";
            }
            if (string.IsNullOrEmpty(txtGatuadress.Text))
            {
                message += "Gatuadress\n";
            }
            if (string.IsNullOrEmpty(txtPostnr.Text))
            {
                message += "Postnummer\n";
            }
            if (string.IsNullOrEmpty(txtPostadress.Text))
            {
                message += "Postadress\n";
            }
            if (string.IsNullOrEmpty(txtTelefonnr.Text))
            {
                message += "Telefonnummer\n";
            }
            return message;
        }

        /// <summary>
        /// Checks if user has entered name and address.
        /// </summary>
        /// <returns></returns>
        private bool NameAddressIsGiven()
        {
            if (string.IsNullOrEmpty(MissingInfoToString()))
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Writes contact to text file.
        /// </summary>
        private void WriteToFile()
        {
            StreamWriter sw = new StreamWriter(path, true);
            sw.WriteLine(ContactInfo());
            sw.Close();
            ClearTextboxes();
        }

        private bool EmailExists(string email)
        {
            foreach (var line in lines)
            {
                if (line.Contains(txtEmail.Text))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Adds contact to text file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {

            if (EmailIsValid(txtEmail.Text))
            {
                if (!EmailExists(txtEmail.Text))
                {
                    if (NameAddressIsGiven())
                    {
                        WriteToFile();
                    }
                    else
                    {
                        DialogResult dlg = MessageBox.Show("Följande fält saknas:\n" + MissingInfoToString() + "Vill du spara ändå?",
                            "Fält saknas", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dlg == DialogResult.Yes)
                        {
                            WriteToFile();
                        }
                    }
                }
                else
                    MessageBox.Show("Emailadressen finns redan, det går inte att spara");
            }
            else
            {
                MessageBox.Show("Du har angett en felaktig mailadress.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        /// <summary>
        /// Creates a List of all addresses.
        /// </summary>
        public void AddressList()
        {
            var fileCheckOK = File.Exists(path) && new FileInfo(path).Length > 0;

            if(fileCheckOK)
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    
                    while ((line = sr.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
            }
         }

        /// <summary>
        /// Clears listbox.
        /// </summary>
        public void EmptyListBox()
        {
            lstAdresser.DataSource = null;
            lstAdresser.Items.Clear();
            
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            lines.Clear();
            AddressList();
            lstAdresser.DataSource = SearchAddressBook();
        }

        private List<string> SearchAddressBook()
        {
            List<string> searchList = new List<string>();

            foreach (var line in lines)
            {
                if (line.ToLower().Contains(txtSok.Text.ToLower()))
                {
                    searchList.Add(line);
                }
            }

            EmptyListBox();

            return searchList;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstAdresser.SelectedIndex >= 0)
            {
                var selectedPerson = lstAdresser.SelectedItem.ToString();
                lines.Remove(selectedPerson);
                File.WriteAllLines(path, lines);
                lstAdresser.DataSource = null;
                lstAdresser.DataSource = SearchAddressBook();
                ClearTextboxes();
            }
        }

        private void lstAdresser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstAdresser.SelectedItem != null)
            {
                string rad = lstAdresser.SelectedItem.ToString();
                
                var kontaktinfo = rad.Split(',');
                txtNamn.Text = kontaktinfo[0].Trim();
                txtGatuadress.Text = kontaktinfo[1].Trim();
                txtPostnr.Text = kontaktinfo[2].Trim();
                txtPostadress.Text = kontaktinfo[3].Trim();
                txtTelefonnr.Text = kontaktinfo[4].Trim();
                txtEmail.Text = kontaktinfo[5].Trim();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (lstAdresser.SelectedValue == null)
                return;

            var selectedValue = lstAdresser.SelectedValue.ToString();
            var newLines = lines.Select(textValue => textValue.Equals(selectedValue) ?
                            textValue.Replace(selectedValue, ContactInfo()) : textValue);
            
            File.WriteAllLines(path, newLines);

            lines.Clear();
            AddressList();
            EmptyListBox();
            lstAdresser.DataSource = lines;
            ClearTextboxes();

        }
    }
}
