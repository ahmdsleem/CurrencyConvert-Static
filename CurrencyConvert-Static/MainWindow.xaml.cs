using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace CurrencyConvert_Static
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection con = new SqlConnection();    // Create Object for SqlConnection
        SqlCommand cmd = new SqlCommand();          // Create Object for SqlCommand
        SqlDataAdapter da = new SqlDataAdapter();   // Create Object for SqlDataAdapter

        private int CurrencyId = 0;     // Declare CurrencyId with int DataType and Assin Value 0
        private double FromAmount = 0;  // Declare FromAmount with double DataType and Assign Value 0
        private double ToAmount = 0;    // Declare ToAmount with double DataType and Assign Value 0

        public MainWindow()
        {
            InitializeComponent();
            BindCurrency();
            GetData();
        }
        // CRUD (Create Read Delete Update) excution
        public void mycon()
        {
            String Conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;  // DataBase connection String
            con = new SqlConnection(Conn);
            con.Open(); // Connection Open
        }

        private void BindCurrency()
        {
            mycon();
            // Create an object for DataTable
            DataTable dt = new DataTable();
            // Write query to get data from Currency_Master table
            cmd = new SqlCommand("select Id, CurrencyName from Currency_Master", con);
            // CommandType define which type of command we use for write a query
            cmd.CommandType = CommandType.Text;

            // It is accepting a parameter that contains the command text of the object's slectCommand property
            da = new SqlDataAdapter(cmd);

            da.Fill(dt);

            // Create an object for DataRow
            DataRow newRow = dt.NewRow();
            // Assign a value to Id column
            newRow["Id"] = 0;
            // Assign a value to CurrencyName column
            newRow["CurrencyName"] = "--SELECT--";

            // Insert a new row in dt with the data at a 0 position
            dt.Rows.InsertAt(newRow, 0);

            // dt is not null and rows count greater than 0
            if(dt != null && dt.Rows.Count > 0)
            {
                // Assign the datatable data to from currency combobox using ItemSource property.
                cmbFromCurrency.ItemsSource = dt.DefaultView;

                // Assign the datatable data to currency combobox using ItemSource property
                cmbToCurrency.ItemsSource = dt.DefaultView;
            }
            con.Close();

            //The data to currency Combobox is assigned from datatable

            //DisplayMemberPath Property is used to display data in Combobox
            cmbFromCurrency.DisplayMemberPath = "CurrencyName";
            //SelectedValuePath property is used to set the value in Combobox
            cmbFromCurrency.SelectedValuePath = "Id";
            //SelectedIndex property is used to bind hint in the Combobox. The default value is Select.
            cmbFromCurrency.SelectedIndex = 0;

            //All properties are set for 'To Currency' Combobox as 'From Currency' Combobox
            cmbToCurrency.DisplayMemberPath = "CurrencyName";
            cmbToCurrency.SelectedValuePath = "Id";
            cmbToCurrency.SelectedIndex = 0;
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            // Create a variable as ConvertedValue with double data type to store currency converted value
            double convertedValue;

            // Check aboumt textbox is Nul or Blank
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                // If amount textbox is Null or Blank it will show the below message box
                MessageBox.Show("Please Enter Curreny", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                // After clicking on message box OK sets the Focus on amuont textbox
                txtCurrency.Focus();
                return;
            }
            // Else if the currency from is not selected or it is defaulttext --SELECTE--
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                // It will show the message
                MessageBox.Show("Please Select currency from", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                // Set focus on From Combobox
                cmbFromCurrency.Focus();
                return;
            }
            // Else if currency To is not selected or select Default Text --SELECTE--
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                // It will show the message
                MessageBox.Show("Please select currency to", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                // Set focus on to combogox
                cmbToCurrency.Focus();
                return;
            }

            // If from and To Combobox selected values are same
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                // The amount textbox value set in ConvertedValue.
                // double.parse is used to convert datatype String To Double.
                // Textbox text have string and convertedValue is double datatype
                convertedValue = double.Parse(txtCurrency.Text);

                // Show in label converted currency and coverted currency name.
                // and ToString("N3") is used to place 000 after the(.)
                lblCurrency.Content = cmbToCurrency.Text + " " + convertedValue.ToString("N3");
            }
            else
            {
                // Calculation for currency converter is From Currency value multiply(*)
                // with amount textbox value and then teh total is divided(/) with To Currency value
                convertedValue = (double.Parse(cmbFromCurrency.SelectedValue.ToString()) *
                    double.Parse(txtCurrency.Text)) / double.Parse(cmbToCurrency.SelectedValue.ToString());

                // Show in label converted currency and converted currency name.
                lblCurrency.Content = cmbToCurrency.Text + " " + convertedValue.ToString("N3");
            }
        }
        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if (cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }

        // Clear button clic event
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            // ClearControls method is used to clear all control value
            ClearControls();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtAmount.Text == null || txtAmount.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtAmount.Focus();
                    return;
                }
                else if (txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter currency name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrencyName.Focus();
                    return;
                }
                else
                {
                    if (CurrencyId > 0)      // Code for Update button. Here Check CurrencyId greater than zero than it is go for update
                    {
                        if (MessageBox.Show("Are you sure you want to update ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)  // Show Confirmation message
                        {
                            mycon();
                            DataTable dt = new DataTable();
                            cmd = new SqlCommand("UPDATE Currency_Master SET Amount = @Amount, CurrencyName = @CurrencyName WHERE Id = @Id", con);  // Update Query Record update using Id
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Id", CurrencyId);
                            cmd.Parameters.AddWithValue("Amount", txtAmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            con.Close();

                            MessageBox.Show("Data updated successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else    // Save Button Code
                    {
                        if (MessageBox.Show("Are you sure you want to save ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mycon();
                            cmd = new SqlCommand("INSERT INTO Currency_Master(Amount, CurrencyName) VALUES(@Amount, @CurrencyName)", con);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            con.Close();

                            MessageBox.Show("Data saved successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    ClearMaster();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ClearMaster()  // This Method is Used to Clear All the input Which the User Entered in Currency Master tab
        {
            try
            {
                txtAmount.Text = string.Empty;
                txtCurrencyName.Text = string.Empty;
                btnSave.Content = "Save";
                GetData();
                CurrencyId = 0;
                BindCurrency();
                txtAmount.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void GetData()   // Bind Data in DataGrid
        {
            mycon();    // mycon() method is used for connect with database and open database connection
            DataTable dt = new DataTable();     // Create Datatable object
            cmd = new SqlCommand("SELECT * FROM Currency_Master", con);     // Write Sql Query for Get data from database table. Query written in double quotes and after comma provide sonnection
            cmd.CommandType = CommandType.Text;     // CommandType define which type of command execute like Text, StoredProcedure, TableDirect.
            da = new SqlDataAdapter(cmd);       // It is accept a parameter that contains the command text of the object's SelectCommand property.
            da.Fill(dt);    // The DataAdapter serves as a bridge between a DataSet and a data source for retrieving and saving data. The Fill operation then adds the rows to destination DataTable objects in the DataSet.
            if (dt != null && dt.Rows.Count > 0)    // dt is not null and rows count greater than 0
            {
                dgvCurrency.ItemsSource = dt.DefaultView;   // Assign DataTable data to dgvCurrency using ItemSource Property.
            }
            else
            {
                dgvCurrency.ItemsSource = null;
                con.Close();    // Database connection Close
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearMaster();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void dgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                DataGrid grd = (DataGrid)sender;        // Create object for DataGrid
                DataRowView row_selected = grd.CurrentItem as DataRowView;  // Create object for DataRowView

                if(row_selected != null)
                {
                    if(dgvCurrency.Items.Count > 0)     // dgvCurrency items count greater than zero
                    {
                        if(grd.SelectedCells.Count > 0)
                        {
                            CurrencyId = Int32.Parse(row_selected["Id"].ToString());    // Get selected row Id column value and Set in CurrencyId variable

                            if (grd.SelectedCells[0].Column.DisplayIndex == 0)  // DisplayIndex is equal to zero than it is Edit cell
                            {
                                txtAmount.Text = row_selected["Amount"].ToString();     // Get selected row Amount column value and Set in Amount textbox
                                txtCurrencyName.Text = row_selected["CurrencyName"].ToString();     // Get selected row CurrencyName column value and Set in CurrencyName textbox
                                btnSave.Content = "Update";     // Change save button text Save to Update
                            }
                            if (grd.SelectedCells[0].Column.DisplayIndex == 1)     // DisplayIndex is equal to one than it is Delete cell
                            {
                                if(MessageBox.Show("Are you sure you want to delte ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    mycon();
                                    DataTable dt = new DataTable();
                                    cmd = new SqlCommand("DELETE FROM Currency_Master WHERE Id = @Id", con);    // Execute delete query for delete record from table using
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.AddWithValue("@Id", CurrencyId);     // CurrencyId set in @Id parameter and send it in delete statement
                                    cmd.ExecuteNonQuery();
                                    con.Close();

                                    MessageBox.Show("Data deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                    ClearMaster();
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
