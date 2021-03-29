﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using SnoopAutoCADCSharp.Model;
using SnoopAutoCADCSharp.ViewModel;
using Exception = System.Exception;

namespace SnoopAutoCADCSharp.View
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SnoopViewModel _viewModel;
        public MainWindow(SnoopViewModel vm)
        {
            InitializeComponent();
            this._viewModel = vm;
            this.DataContext = vm;
            SnoopViewModel.FrmMain = this;

        }

        private void Treeview_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {

                TreeViewCustomItem selected = e.NewValue as TreeViewCustomItem;
                object obj = selected.Object;
                DBObject dbObject = obj as DBObject;
                if (dbObject != null)
                {
                    foreach (KeyValuePair<string, List<ObjectDetails>> pair in _viewModel.DataMethod)
                    {
                        if (pair.Key == dbObject.Id.ToString())
                        {
                            _viewModel.LisViewItems = new List<ObjectDetails>();
                            _viewModel.LisViewItems = pair.Value;
                        }
                    }

                    _viewModel.UpdateDataSource(_viewModel.LisViewItems);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.StackTrace);
            }
        }

        private void ListViewItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.listview.SelectedItems.Count != 1)
            {
                return;
            }
            ObjectDetails selectedItem = this.listview.SelectedItem as ObjectDetails;
            object LinkObject = selectedItem.LinkObject;
            if (_viewModel.IsEnumerable(LinkObject))
            {
                _viewModel.CollectionItemSelected(LinkObject);
            }
            else if (LinkObject is ObjectId)
            {
                _viewModel.ObjectIdItemSelected(LinkObject);
            }
        }


        private void ContextMenu_MouseDown(object sender, RoutedEventArgs e)
        {
            MenuItem menuitem = sender as MenuItem;
            if (menuitem != null)
            {
                ContextMenu parent_contextmenu = menuitem.CommandParameter as ContextMenu;
                if (parent_contextmenu != null)
                {
                    string clip = "";
                    foreach (var item in this.listview.SelectedItems)
                    {
                        ObjectDetails objectDetails = item as ObjectDetails;
                        clip += objectDetails.PropName + "\t" + objectDetails.Type + "\t" + objectDetails.Value + "\n";
                    }
                    Clipboard.SetText(clip);
                }
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}

