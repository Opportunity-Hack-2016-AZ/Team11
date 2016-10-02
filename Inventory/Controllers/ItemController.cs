﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Inventory.Models;
using MySql.Data.MySqlClient;

namespace Inventory.Controllers
{
    public class ItemController : Controller
    {

        // GET: /Category/
        public ActionResult Index()
        {
            var strConnString = ConfigurationManager.ConnectionStrings["Development"].ConnectionString;

            MySqlConnection mconn = new MySqlConnection(strConnString);
            mconn.Open();

            MySqlCommand command = mconn.CreateCommand();
            command.CommandText = "select * from Items";
            MySqlDataReader reader = command.ExecuteReader();
            var result = new List<Item>();
            while (reader.Read())
            {
                result.Add(new Item() { CategoryId = reader.GetInt32(0), ItemId = reader.GetInt32(1), Name = reader.GetString(2), Gender = reader.GetString(3), Size = reader.GetString(4), Age = reader.GetString(5), LowLimit = reader.GetInt32(6), Price = reader.GetDecimal(7)});
            }

            reader.Close();
            return View(result);
        }

        private Item GetItem(int id)
        {
            var strConnString = ConfigurationManager.ConnectionStrings["Development"].ConnectionString;

            MySqlConnection mconn = new MySqlConnection(strConnString);
            mconn.Open();

            MySqlCommand command = mconn.CreateCommand();
            command.CommandText = "select * from Items where ItemId = " + id;
            MySqlDataReader reader = command.ExecuteReader();
            
            var item = new Item();

            while (reader.Read())
            {
                item.CategoryId = reader.GetInt32(0);
                item.ItemId = reader.GetInt32(1);
                item.Name = reader.GetString(2);
                item.Gender = reader.GetString(3);
                item.Size = reader.GetString(4);
                item.Age = reader.GetString(5);
                item.LowLimit = reader.GetInt32(6);
                item.Price = reader.GetDecimal(7);
            }

            reader.Close();
            return item;
        }

        public ActionResult Create()
        {
            var newItem = new Item();
            return View(newItem);
        }

        [HttpPost]
        public ActionResult Create(Item item)
        {
            CheckForDuplicate(item);

            if (!ModelState.IsValid)
                return View(item);
            var strConnString = ConfigurationManager.ConnectionStrings["Development"].ConnectionString;

            MySqlConnection mconn = new MySqlConnection(strConnString);
            try
            {
                mconn.Open();

                MySqlCommand command = mconn.CreateCommand();
                command.CommandText = "Insert into Items (ItemId) values ('"+item.ItemId+"')";
                var output=command.ExecuteNonQuery();
                if (output != 1) {
                }
            }
            catch (Exception e) { }
            finally {
                if (mconn.State == System.Data.ConnectionState.Open)
                    mconn.Close();
            }

                return RedirectToAction("Index");
        }

        private void CheckForDuplicate(Item item)
        {            
                if (item == null)
                    return;

                var strConnString = ConfigurationManager.ConnectionStrings["Development"].ConnectionString;

                MySqlConnection mconn = new MySqlConnection(strConnString);
            try
            {
                mconn.Open();

                MySqlCommand command = mconn.CreateCommand();
                command.CommandText = "select count(*) from Items where ItemId = " + item.ItemId;

               int rowscount = Convert.ToInt32(command.ExecuteScalar());
               
                if (rowscount > 0)
                    ModelState.AddModelError("Item ID", "An item with this name already exists.");
            }
            catch (Exception e)
            {
                
            }
            finally {
                if(mconn.State == System.Data.ConnectionState.Open)
                    mconn.Close();   
            }
        }

        public ActionResult Edit(int id)
        {
            var item = GetItem(id);
            return View(item);
        }

        [HttpPost]
        public ActionResult Edit(Item item)
        {
            CheckForDuplicate(item);

            if (!ModelState.IsValid)
                return View(item);

           var strConnString = ConfigurationManager.ConnectionStrings["Development"].ConnectionString;

            MySqlConnection mconn = new MySqlConnection(strConnString);
            try
            {
                mconn.Open();

                MySqlCommand command = mconn.CreateCommand();
                command.CommandText = "Update Items Set Name = '" + item.Name + "' Where ItemId = " + item.ItemId;
                var output = command.ExecuteNonQuery();
                if (output != 1)
                {
                }
            }
            catch (Exception e) { }
            finally
            {
                if (mconn.State == System.Data.ConnectionState.Open)
                    mconn.Close();
            }

            return RedirectToAction("Index");
        }
    }

}