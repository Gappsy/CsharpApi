using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CsharpApi
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient client = new HttpClient();

        public Form1()
        {
            InitializeComponent();
        }

        private async void btnGet_Click(object sender, EventArgs e)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost/myapi/phpapi/api.php");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                List<AccountData> accounts = JsonConvert.DeserializeObject<List<AccountData>>(responseBody);
                account_grid.DataSource = null;
                account_grid.DataSource = accounts;

                account_grid.Columns["UserId"].HeaderText = "User ID";
                account_grid.Columns["Username"].HeaderText = "Username";
                account_grid.Columns["Email"].HeaderText = "Email";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private async void btnPost_Click(object sender, EventArgs e)
        {
            var userData = new { username = txtUsername.Text, pass = txtPassword.Text, email = txtEmail.Text };
            string json = JsonConvert.SerializeObject(userData);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync("http://localhost/myapi/phpapi/api.php", content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                MessageBox.Show(responseBody); // Show server response
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private async void btnGetEmp_Click(object sender, EventArgs e)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost/myapi/phpapi/api.php?action=get_employees");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize JSON data into a List of EmployeeData objects
                List<EmployeeData> employees = JsonConvert.DeserializeObject<List<EmployeeData>>(responseBody);

                // Clear and populate employee_grid with retrieved data
                employee_grid.DataSource = null;
                employee_grid.DataSource = employees;


                employee_grid.Columns["Id"].HeaderText = "Employee ID";
                employee_grid.Columns["Name"].HeaderText = "Name";
                employee_grid.Columns["Salary"].HeaderText = "Salary";
                employee_grid.Columns["Bdate"].HeaderText = "Birthdate";  
                employee_grid.Columns["Department"].HeaderText = "Department";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private async void btnPostEmp_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtDNo.Text) || string.IsNullOrEmpty(txtSalary.Text))
            {
                MessageBox.Show("Please fill in all employee details.");
                return;
            }

            var empData = new { action="add_employee", name = txtName.Text, dno = int.Parse(txtDNo.Text), salary = double.Parse(txtSalary.Text) };

            // Serialize to JSON
            string json = JsonConvert.SerializeObject(empData);

            // Create HTTP content with JSON data and application/json content type
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                
                HttpResponseMessage response = await client.PostAsync("http://localhost/myapi/phpapi/api.php?action=add_employee", content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                MessageBox.Show(responseBody); 

                
                employee_grid.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}

    public class AccountData
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }

    public class EmployeeData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double? Salary { get; set; }
        public DateTime? Bdate { get; set; } 
        public string Department { get; set; }
    }


