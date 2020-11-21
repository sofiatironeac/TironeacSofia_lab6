using AutoLotModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
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

namespace TironeacSofia_lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        CollectionViewSource customerViewSource;
        CollectionViewSource inventoryViewSource;
        CollectionViewSource customerOrdersViewSource;
        Binding txtFirstNameBinding = new Binding();
        Binding txtLastNameBinding = new Binding();
        Binding carIdTextBoxBinding = new Binding();
        Binding colorTextBoxBinding = new Binding();
        Binding makeTextBoxBinding = new Binding();
        Binding cmbInventoryBinding = new Binding();
        Binding cmbCustomersBinding = new Binding();



        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            txtFirstNameBinding.Path = new PropertyPath("FirstName");
            txtLastNameBinding.Path = new PropertyPath("LastName");
            firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
            colorTextBoxBinding.Path = new PropertyPath("Color");
            makeTextBoxBinding.Path = new PropertyPath("Make");
            colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
            cmbInventoryBinding.Path = new PropertyPath("Make");
            cmbCustomersBinding.Path = new PropertyPath("FirstName");

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;

            inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            inventoryViewSource.Source = ctx.Inventories.Local;

            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));
            //customerOrdersViewSource.Source = ctx.Orders.Local;

            ctx.Customers.Load();
            ctx.Inventories.Load();
            ctx.Orders.Load();

            cmbCustomers.ItemsSource = ctx.Customers.Local;
            //cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomers.SelectedValuePath = "CustId";

            cmbInventory.ItemsSource = ctx.Inventories.Local;
           //cmbInventory.DisplayMemberPath = "Make";
            cmbInventory.SelectedValuePath = "CarId";

            // Load data by setting the CollectionViewSource.Source property:
            // inventoryViewSource.Source = [generic data source]

            BindDataGrid();
        }

        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals
                             cust.CustId
                             join inv in ctx.Inventories on ord.CarId
                 equals inv.CarId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CarId,
                                 ord.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 inv.Make,
                                 inv.Color
                             };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }

        private void btnSaveCust_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                btnNewCust.IsEnabled = true;
                btnEditCust.IsEnabled = true;
                btnDeleteCust.IsEnabled = true;

                btnSaveCust.IsEnabled = false;
                btnCancelCust.IsEnabled = false;

                customerDataGrid.IsEnabled = true;

                btnPrevCust.IsEnabled = true;
                btnNextCust.IsEnabled = true;

                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;

            }
            else
            {
                if (action == ActionState.Edit)
                {
                    try
                    {
                        customer = (Customer)customerDataGrid.SelectedItem;
                        customer.FirstName = firstNameTextBox.Text.Trim();
                        customer.LastName = lastNameTextBox.Text.Trim();
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    btnNewCust.IsEnabled = true;
                    btnEditCust.IsEnabled = true;
                    btnDeleteCust.IsEnabled = true;

                    btnSaveCust.IsEnabled = false;
                    btnCancelCust.IsEnabled = false;

                    customerDataGrid.IsEnabled = true;

                    btnPrevCust.IsEnabled = true;
                    btnNextCust.IsEnabled = true;

                    firstNameTextBox.IsEnabled = false;
                    lastNameTextBox.IsEnabled = false;

                    firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
                    lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);

                    //SetValidationBinding();

                    customerViewSource.View.Refresh();
                    // pozitionarea pe item-ul curent
                    customerViewSource.View.MoveCurrentTo(customer);
                }
                else if (action == ActionState.Delete)
                {
                    try
                    {
                        customer = (Customer)customerDataGrid.SelectedItem;
                        ctx.Customers.Remove(customer);
                        ctx.SaveChanges();
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    btnNewCust.IsEnabled = true;
                    btnEditCust.IsEnabled = true;
                    btnDeleteCust.IsEnabled = true;

                    btnSaveCust.IsEnabled = false;
                    btnCancelCust.IsEnabled = false;

                    customerDataGrid.IsEnabled = true;

                    btnPrevCust.IsEnabled = true;
                    btnNextCust.IsEnabled = true;

                    firstNameTextBox.IsEnabled = false;
                    lastNameTextBox.IsEnabled = false;

                    firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
                    lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);

                    //SetValidationBinding();

                    customerViewSource.View.Refresh();
                }
            }
        }

        private void btnNextCust_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }
        private void btnPreviousCust_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }
        private void btnNewCust_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNewCust.IsEnabled = false;
            btnEditCust.IsEnabled = false;
            btnDeleteCust.IsEnabled = false;

            btnSaveCust.IsEnabled = true;
            btnCancelCust.IsEnabled = true;

            customerDataGrid.IsEnabled = false;

            btnPrevCust.IsEnabled = false;
            btnNextCust.IsEnabled = false;

            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            Keyboard.Focus(firstNameTextBox);
        }

        private void btnEditCust_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();

            btnNewCust.IsEnabled = false;
            btnEditCust.IsEnabled = false;
            btnDeleteCust.IsEnabled = false;

            btnSaveCust.IsEnabled = true;
            btnCancelCust.IsEnabled = true;

            customerDataGrid.IsEnabled = false;

            btnPrevCust.IsEnabled = false;
            btnNextCust.IsEnabled = false;

            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
            Keyboard.Focus(firstNameTextBox);

            //SetValidationBinding();
        }

        private void btnDeleteCust_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();

            btnNewCust.IsEnabled = false;
            btnEditCust.IsEnabled = false;
            btnDeleteCust.IsEnabled = false;

            btnSaveCust.IsEnabled = true;
            btnCancelCust.IsEnabled = true;

            customerDataGrid.IsEnabled = false;

            btnPrevCust.IsEnabled = false;
            btnNextCust.IsEnabled = false;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;

        }

        private void btnCancelCust_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNewCust.IsEnabled = true;
            btnEditCust.IsEnabled = true;

            btnSaveCust.IsEnabled = false;
            btnCancelCust.IsEnabled = false;

            customerDataGrid.IsEnabled = true;

            btnPrevCust.IsEnabled = true;
            btnNextCust.IsEnabled = true;

            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;

            firstNameTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
        }

        private void btnSaveInv_Click(object sender, RoutedEventArgs e)
        {
            Inventory car = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Inventory entity
                    car = new Inventory()
                    {
                        Color = colorTextBox.Text.Trim(),
                        Make = makeTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Inventories.Add(car);
                    inventoryViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                btnNewInv.IsEnabled = true;
                btnEditInv.IsEnabled = true;
                btnDeleteInv.IsEnabled = true;

                btnSaveInv.IsEnabled = false;
                btnCancelInv.IsEnabled = false;

                inventoryDataGrid.IsEnabled = true;

                btnPrevInv.IsEnabled = true;
                btnNextInv.IsEnabled = true;

                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;

            }
            else
            {
                if (action == ActionState.Edit)
                {
                    try
                    {
                        car = (Inventory)inventoryDataGrid.SelectedItem;
                        car.Color = colorTextBox.Text.Trim();
                        car.Make = makeTextBox.Text.Trim();
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    btnNewInv.IsEnabled = true;
                    btnEditInv.IsEnabled = true;
                    btnDeleteInv.IsEnabled = true;

                    btnSaveInv.IsEnabled = false;
                    btnCancelInv.IsEnabled = false;

                    inventoryDataGrid.IsEnabled = true;

                    btnPrevInv.IsEnabled = true;
                    btnNextInv.IsEnabled = true;

                    colorTextBox.IsEnabled = false;
                    makeTextBox.IsEnabled = false;

                    colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);
                    makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);

                    //SetValidationBinding();

                    inventoryViewSource.View.Refresh();
                    // pozitionarea pe item-ul curent
                    inventoryViewSource.View.MoveCurrentTo(car);
                }
                else if (action == ActionState.Delete)
                {
                    try
                    {
                        car = (Inventory)inventoryDataGrid.SelectedItem;
                        ctx.Inventories.Remove(car);
                        ctx.SaveChanges();
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    btnNewInv.IsEnabled = true;
                    btnEditInv.IsEnabled = true;
                    btnDeleteInv.IsEnabled = true;

                    btnSaveInv.IsEnabled = false;
                    btnCancelInv.IsEnabled = false;

                    inventoryDataGrid.IsEnabled = true;

                    btnPrevInv.IsEnabled = true;
                    btnNextInv.IsEnabled = true;

                    colorTextBox.IsEnabled = false;
                    makeTextBox.IsEnabled = false;

                    colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);
                    makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);

                    //SetValidationBinding();

                    inventoryViewSource.View.Refresh();
                }
            }
        }

        private void btnNextInv_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToNext();
        }
        private void btnPreviousInv_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToPrevious();
        }
        private void btnNewInv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNewInv.IsEnabled = false;
            btnEditInv.IsEnabled = false;
            btnDeleteInv.IsEnabled = false;

            btnSaveInv.IsEnabled = true;
            btnCancelInv.IsEnabled = true;

            inventoryDataGrid.IsEnabled = false;

            btnPrevInv.IsEnabled = false;
            btnNextInv.IsEnabled = false;

            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            colorTextBox.Text = "";
            makeTextBox.Text = "";
            Keyboard.Focus(makeTextBox);
        }

        private void btnEditInv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempColor = colorTextBox.Text.ToString();
            string tempMake = makeTextBox.Text.ToString();

            btnNewInv.IsEnabled = false;
            btnEditInv.IsEnabled = false;
            btnDeleteInv.IsEnabled = false;

            btnSaveInv.IsEnabled = true;
            btnCancelInv.IsEnabled = true;

            inventoryDataGrid.IsEnabled = false;

            btnPrevInv.IsEnabled = false;
            btnNextInv.IsEnabled = false;

            colorTextBox.IsEnabled = true;
            makeTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            colorTextBox.Text = tempColor;
            makeTextBox.Text = tempMake;
            Keyboard.Focus(makeTextBox);

            //SetValidationBinding();
        }

        private void btnDeleteInv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempMake = makeTextBox.Text.ToString();
            string tempColor = colorTextBox.Text.ToString();

            btnNewInv.IsEnabled = false;
            btnEditInv.IsEnabled = false;
            btnDeleteInv.IsEnabled = false;

            btnSaveInv.IsEnabled = true;
            btnCancelInv.IsEnabled = true;

            inventoryDataGrid.IsEnabled = false;

            btnPrevInv.IsEnabled = false;
            btnNextInv.IsEnabled = false;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            colorTextBox.Text = tempColor;
            makeTextBox.Text = tempMake;

        }

        private void btnCancelInv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNewInv.IsEnabled = true;
            btnEditInv.IsEnabled = true;
            btnDeleteInv.IsEnabled = true;

            btnSaveInv.IsEnabled = false;
            btnCancelInv.IsEnabled = false;

            inventoryDataGrid.IsEnabled = true;

            btnPrevInv.IsEnabled = true;
            btnNextInv.IsEnabled = true;

            colorTextBox.IsEnabled = false;
            makeTextBox.IsEnabled = false;

            colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);
            makeTextBox.SetBinding(TextBox.TextProperty, makeTextBoxBinding);
        }

        private void btnSaveOrd_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Inventory car = (Inventory)cmbInventory.SelectedItem;
                    //instantiem Order entity
                    order = new Order()
                    {
                        CustId = customer.CustId,
                        CarId = car.CarId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Orders.Add(order);
                    customerOrdersViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
        
        

                btnNewOrd.IsEnabled = true;
                btnEditOrd.IsEnabled = true;
                btnDeleteOrd.IsEnabled = true;

                btnSaveOrd.IsEnabled = false;
                btnCancelOrd.IsEnabled = false;

                ordersDataGrid.IsEnabled = true;

                btnPrevOrd.IsEnabled = true;
                btnNextOrd.IsEnabled = true;

                cmbCustomers.IsEnabled = true;
                cmbInventory.IsEnabled = true;

             
            }
            else
            {
                if (action == ActionState.Edit)
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;
                    try
                    {
                        int curr_id = selectedOrder.OrderId;
                        var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                        if (editedOrder != null)
                        {
                            editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                            editedOrder.CarId = Convert.ToInt32(cmbInventory.SelectedValue.ToString());
                            //salvam modificarile
                            ctx.SaveChanges();
                        }
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    BindDataGrid();
                    // pozitionarea pe item-ul curent
                    customerViewSource.View.MoveCurrentTo(selectedOrder);

                    btnNewOrd.IsEnabled = true;
                    btnEditOrd.IsEnabled = true;
                    btnDeleteOrd.IsEnabled = true;

                    btnSaveOrd.IsEnabled = false;
                    btnCancelOrd.IsEnabled = false;

                    ordersDataGrid.IsEnabled = true;

                    btnPrevOrd.IsEnabled = true;
                    btnNextOrd.IsEnabled = true;

                    cmbCustomers.IsEnabled = false;
                    cmbInventory.IsEnabled = false;

                }
            

               
                else if (action == ActionState.Delete)
                {
                    try
                    {
                        dynamic selectedOrder = ordersDataGrid.SelectedItem;
                        int curr_id = selectedOrder.OrderId;
                        var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                        if (deletedOrder != null)
                        {
                            ctx.Orders.Remove(deletedOrder);
                            ctx.SaveChanges();
                            MessageBox.Show("Order Deleted Successfully", "Message");
                            BindDataGrid();
                        }
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    btnNewOrd.IsEnabled = true;
                    btnEditOrd.IsEnabled = true;
                    btnDeleteOrd.IsEnabled = true;

                    btnSaveOrd.IsEnabled = false;
                    btnCancelOrd.IsEnabled = false;

                    ordersDataGrid.IsEnabled = true;

                    btnPrevOrd.IsEnabled = true;
                    btnNextOrd.IsEnabled = true;

                    cmbCustomers.IsEnabled = false;
                    cmbInventory.IsEnabled = false;

                    SetValidationBinding();
                }
                
            }
        }

        private void btnNextOrd_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }
        private void btnPreviousOrd_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }
        private void btnNewOrd_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNewOrd.IsEnabled = false;
            btnEditOrd.IsEnabled = false;
            btnDeleteOrd.IsEnabled = false;

            btnSaveOrd.IsEnabled = true;
            btnCancelOrd.IsEnabled = true;

            ordersDataGrid.IsEnabled = false;

            btnPrevInv.IsEnabled = false;
            btnNextInv.IsEnabled = false;

            cmbCustomers.IsEnabled = true;
            cmbInventory.IsEnabled = true;

            SetValidationBinding();

            BindingOperations.ClearBinding(cmbCustomers, ComboBox.TextProperty);
            BindingOperations.ClearBinding(cmbInventory, ComboBox.TextProperty);
            cmbCustomers.Text = "";
            cmbInventory.Text = "";
            Keyboard.Focus(cmbCustomers);

        }

        private void btnEditOrd_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            
            btnNewOrd.IsEnabled = false;
            btnEditOrd.IsEnabled = false;
            btnDeleteOrd.IsEnabled = false;

            btnSaveOrd.IsEnabled = true;
            btnCancelOrd.IsEnabled = true;

            ordersDataGrid.IsEnabled = false;

            btnPrevOrd.IsEnabled = false;
            btnNextOrd.IsEnabled = false;

            cmbCustomers.IsEnabled = true;
            cmbInventory.IsEnabled = true;

            SetValidationBinding();

            BindingOperations.ClearBinding(cmbCustomers, ComboBox.TextProperty);
            BindingOperations.ClearBinding(cmbInventory, ComboBox.TextProperty);
            Keyboard.Focus(cmbCustomers);
           
        }

        private void btnDeleteOrd_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            btnNewOrd.IsEnabled = false;
            btnEditOrd.IsEnabled = false;
            btnDeleteOrd.IsEnabled = false;

            btnSaveOrd.IsEnabled = true;
            btnCancelOrd.IsEnabled = true;

            ordersDataGrid.IsEnabled = false;

            btnPrevOrd.IsEnabled = false;
            btnNextOrd.IsEnabled = false;

            cmbCustomers.IsEnabled = false;
            cmbInventory.IsEnabled = false;

            BindingOperations.ClearBinding(cmbCustomers, ComboBox.TextProperty);
            BindingOperations.ClearBinding(cmbInventory, ComboBox.TextProperty);
           
        }

        private void btnCancelOrd_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNewOrd.IsEnabled = true;
            btnEditOrd.IsEnabled = true;
            btnDeleteOrd.IsEnabled = true;

            btnSaveOrd.IsEnabled = false;
            btnCancelOrd.IsEnabled = false;

            ordersDataGrid.IsEnabled = true;

            btnPrevOrd.IsEnabled = true;
            btnNextOrd.IsEnabled = true;

            cmbCustomers.IsEnabled = false;
            cmbInventory.IsEnabled = false;


        }
        private void SetValidationBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string required
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameValidationBinding);
            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string min length validator
            lastNameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameValidationBinding); //setare binding nou
        }



    }
}
