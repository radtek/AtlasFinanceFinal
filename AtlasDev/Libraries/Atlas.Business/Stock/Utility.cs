using Atlas.Common.Attributes;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Atlas.Business.Stock
{
  public static class Utility
  {
    #region public methods

    /// <summary>
    /// Convert DataTable into an IList then calls Save to DB
    /// </summary>
    /// <param name="data">The Data Table of the products</param>
    /// <param name="productType">Type of Product</param>
    /// <param name="batchStatus">Status of the Batch</param>
    /// <param name="courierCompany">Courier Company for Delivery</param>
    /// <param name="captureUser">User that capture the products</param>
    /// <param name="quantity">Number of Products in the new batch</param>
    /// <param name="comment">Comment made by capturer</param>
    /// <param name="deliveryBranch">branch the Batch is o be delivered to</param>
    /// <param name="deliveryDate">Date of Delivery</param>
    /// <param name="trackingNum">Traking Number provided by courier company</param>
    public static void SaveProducts(DataTable data, ProductTypeDTO productType, Enumerators.General.ProductBatchStatus batchStatus, CPY_CompanyDTO courierCompany,
      PER_PersonDTO captureUser, int quantity, string comment, BranchCompany deliveryBranch = null, DateTime? deliveryDate = null,
      string trackingNum = null)
    {
      var productObjList = ConvertToProduct(data, productType.AssemblyName);

      ProductManager.CaptureProducts(productObjList, productType, batchStatus, courierCompany,
       captureUser, quantity, comment, deliveryBranch, deliveryDate, trackingNum);
    }

    /// <summary>
    /// Converts DataTable into object and  saves to DB
    /// </summary>
    /// <param name="data"></param>
    /// <param name="productType"></param>
    /// <param name="productDTO"></param>
    /// <param name="editingUser"></param>
    public static string UpdateProduct(DataTable data, ProductTypeDTO productType, long productId, PER_PersonDTO editingUser)
    {
      var objProduct = ConvertToProduct(data, productType.AssemblyName)[0];
      var searchValues = GetSearchValues(objProduct);

      var xmlObject =  Serialize(objProduct.GetType(), objProduct, true);
      var searchValue1 = searchValues[0];
      var searchValue2 = searchValues[1];

      ProductManager.UpdateProduct(productId, xmlObject, searchValue1, searchValue2, editingUser);

      return xmlObject;
    }

    /// <summary>
    /// Convert List of Products from XML to DataTable 
    /// </summary>
    /// <param name="productDetails">List to Convert</param>
    /// <param name="assemblyName">full name of the Type of product the list contains</param>
    /// <returns>Converted DataTable</returns>
    public static DataTable GetProductDetails(List<ProductDTO> productDetails, string assemblyName)
    {
      var products = new DataTable();

      Type productType = Domain.Serializable.Product.Helper.GetType(assemblyName);

      for (int i = 0; i < productDetails.Count; i++)
      {
        var productDetail = productDetails[i];

        var result = DeSerialize(productType, productDetail.XmlObject);

        if (i == 0)
        {
          BuildProductHeaderSection(ref products, result.GetType());
          BuildProductHeaderSection(ref products, productDetail.GetType());
        }

        var product = products.NewRow();

        foreach (var prop in result.GetType().GetProperties())
        {
          if (products.Columns.Contains(prop.Name))
            product[prop.Name] = GetNullableValue(prop.PropertyType, prop.GetValue(result, null));
        }

        foreach (var prop in productDetail.GetType().GetProperties())
        {
          if (products.Columns.Contains(prop.Name))
          {
            var dummyProduct = new ProductDTO();

            if (prop.Name == GetPropertyName(() => dummyProduct.ProductBatch))
            {
              var productBatch = (ProductBatchDTO)prop.GetValue(productDetail, null);
              product[prop.Name] = productBatch.ProductBatchId;
            }
            else if (prop.Name == GetPropertyName(() => dummyProduct.CreatedBy) ||
              prop.Name == GetPropertyName(() => dummyProduct.LastEditedBy) ||
              prop.Name == GetPropertyName(() => dummyProduct.AllocatedBy))
            {
              var person = (PER_PersonDTO)prop.GetValue(productDetail, null);
              if (person != null)
                product[prop.Name] = person.Security.Username;
            }
            else
            {
              product[prop.Name] = GetNullableValue(prop.PropertyType, prop.GetValue(productDetail, null));
            }
          }
        }

        products.Rows.Add(product);
      }
      return products;
    }

    /// <summary>
    /// Get the Column Description of a property using the Description Attribute
    /// </summary>
    /// <param name="prop">PropertyInfo of the property</param>
    /// <returns>Description/Caption</returns>
    public static string GetPropDescription(PropertyInfo prop)
    {
      var attributes = prop.GetCustomAttributes(true);
      if (attributes.Length > 0)
      {
        for (var j = 0; j < attributes.Length; j++)
        {
          if (attributes[j].GetType() == typeof(DescriptionAttribute))
          {
            return ((DescriptionAttribute)attributes[j]).Description;
          }
        }
      }
      return prop.Name;
    }

    /// <summary>
    /// Returns a True/False depending on if the property requires a scanner to enter the value of the property
    /// </summary>
    /// <param name="prop">PropertyInfo of the Property</param>
    /// <returns>True/False on whether the property requires a scanner to capture the Data</returns>
    public static bool UseScanner(PropertyInfo prop)
    {
      var attributes = prop.GetCustomAttributes(true);
      if (attributes.Length > 0)
      {
        for (var j = 0; j < attributes.Length; j++)
        {
          if (attributes[j].GetType() == typeof(UseScannerAttribute))
          {
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    /// Build the column section of a DataTable from the Propeties of the passed type
    /// </summary>
    /// <param name="products">DataTable being edited</param>
    /// <param name="type">Type of object that columns are added from</param>
    public static void BuildProductHeaderSection(ref DataTable products, Type type)
    {
      if (products == null)
        products = new DataTable();

      for (var i = 0; i < type.GetProperties().Length; i++)
      {
        var prop = type.GetProperties()[i];

        var col = products.Columns.Add(prop.Name, GetNonNullableType(prop.PropertyType));
        col.Caption = ChangeCaption(col.DataType, col.Caption);

        if (type == typeof(ProductDTO))
        {
          var dummyProduct = new ProductDTO();

          if (prop.Name == GetPropertyName(() => dummyProduct.ProductId))
          {
            col.SetOrdinal(0);
            col.Caption = col.Caption.Replace("Id", "Number");
          }
          else if (prop.Name == GetPropertyName(() => dummyProduct.SearchValue1) ||
            prop.Name == GetPropertyName(() => dummyProduct.SearchValue2) ||
            prop.Name == GetPropertyName(() => dummyProduct.XmlObject) ||
            prop.Name == GetPropertyName(() => dummyProduct.AllocatedPerson))
          {
            products.Columns.Remove(col);
          }
          else if (prop.Name == GetPropertyName(() => dummyProduct.ProductBatch))
          {
            col.Caption += " Number";
            col.DataType = typeof(Int64);
          }
          else if (prop.Name == GetPropertyName(() => dummyProduct.CreatedBy) ||
            prop.Name == GetPropertyName(() => dummyProduct.LastEditedBy) ||
            prop.Name == GetPropertyName(() => dummyProduct.AllocatedBy))
          {
            col.DataType = typeof(string);
          }
        }
        else
        {
          var attributes = type.GetProperties()[i].GetCustomAttributes(true);

          if (attributes.Length > 0)
          {
            for (var j = 0; j < attributes.Length; j++)
            {
              if (attributes[j].GetType() == typeof(DescriptionAttribute))
              {
                col.Caption = ((DescriptionAttribute)attributes[j]).Description;
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Decompresses String and deserializes object
    /// </summary>
    /// <param name="type">Type of object</param>
    /// <param name="xmlObject">the object to deserialize</param>
    /// <returns></returns>
    public static object DeSerialize(Type type, string xmlObject)
    {
      return Atlas.Common.Utils.Xml.DeSerialize(type, xmlObject);
    }

    /// <summary>
    /// Serailizes object then compresses string 
    /// </summary>
    /// <param name="type">Type of object</param>
    /// <param name="objProduct">object to compress and serailize</param>
    /// <param name="stripNamespace">Do/dont: strip namespace from serialized object</param>
    /// <returns></returns>
    public static string Serialize(Type type, object objProduct, bool stripNamespace)
    {
      return Common.Utils.Xml.Serialize(type, objProduct, true);
    }

    /// <summary>
    /// Get the SearchValues from an object and returns an array of 2 string with the values in them
    /// </summary>
    /// <param name="product">the product to get SearchValues from</param>
    /// <returns>Array of Search Values</returns>
    public static string[] GetSearchValues(object product)
    {
      var searchValues = new string[2];

      for (var i = 0; i < product.GetType().GetProperties().Length; i++)
      {
        var prop = product.GetType().GetProperties()[i];
        var attributes = product.GetType().GetProperties()[i].GetCustomAttributes(true);

        if (attributes.Length > 0)
        {
          for (var j = 0; j < attributes.Length; j++)
          {
            if (attributes[j].GetType() == typeof(SearchValueAttribute))
            {
              var searchValueNo = ((SearchValueAttribute)attributes[j]).Num;

              if (searchValueNo == 1)
              {
                searchValues[0] = prop.GetValue(product, null).ToString();
              }
              else if (searchValueNo == 2)
              {
                searchValues[1] = prop.GetValue(product, null).ToString();
              }
            }
          }
        }
      }

      return searchValues;
    }

    public static List<Tuple<string, string>> GetUniqueContraints(IList products)
    {
      var uniqueKeys = new List<Tuple<string, string>>();

      foreach (var product in products)
      {
        Tuple<string, string> unique = null;
        for (var i = 0; i < product.GetType().GetProperties().Length; i++)
        {
          var prop = product.GetType().GetProperties()[i];
          var attributes = product.GetType().GetProperties()[i].GetCustomAttributes(true);

          if (attributes.Length > 0)
          {
            for (var j = 0; j < attributes.Length; j++)
            {
              if (attributes[j].GetType() == typeof(IsUniqueAttribute))
              {
                if (prop.GetValue(product, null) == null
                  || string.IsNullOrEmpty(prop.GetValue(product, null).ToString()))
                  throw new Exception(string.Format("Unique Key '{0}' cannot be NULL/Blank", prop.Name));

                for (var k = 0; k < attributes.Length; k++)
                {
                  if (attributes[k].GetType() == typeof(SearchValueAttribute))
                  {
                    var searchValueNo = ((SearchValueAttribute)attributes[k]).Num;
                    
                    if (searchValueNo == 1)
                    {
                      unique = new Tuple<string, string>(prop.GetValue(product, null).ToString(), unique == null ? "" : unique.Item2);
                    }
                    else if (searchValueNo == 2)
                    {
                      unique = new Tuple<string, string>(unique == null ? "" : unique.Item1, prop.GetValue(product, null).ToString());
                    }
                  }
                }
              }
            }
          }
        }
        if (unique != null)
          uniqueKeys.Add(unique);
      }
      return uniqueKeys;
    }

    /// <summary>
    /// Returns the string of a property name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expression">functon of the property name</param>
    /// <returns>the name of the property</returns>
    public static string GetPropertyName<T>(Expression<Func<T>> expression)
    {
      MemberExpression body = (MemberExpression)expression.Body;
      return body.Member.Name;
    }

    #endregion

    #region private methods

    /// <summary>
    /// Create a List<> of unknown type from the DataTable passed
    /// </summary>
    /// <param name="data">The DataTable to Convert</param>
    /// <param name="assemblyName">The Full Name of the type of Ilist to Create</param>
    /// <returns></returns>
    private static IList ConvertToProduct(DataTable data, string assemblyName)
    {
      Type type = Domain.Serializable.Product.Helper.GetType(assemblyName);

      Type customList = typeof(List<>).MakeGenericType(type);
      IList objectList = (IList)Activator.CreateInstance(customList);

      foreach (DataRow dataRow in data.Rows)
      {
        var product = Activator.CreateInstance(type);

        foreach (var prop in product.GetType().GetProperties())
        {
          var colName = prop.Name;
          if (!string.IsNullOrEmpty(dataRow[colName].ToString()))
            prop.SetValue(product, dataRow[colName], null);
        }

        objectList.Add(product);
      }

      return objectList;
    }

    /// <summary>
    /// Changes the Caption Text for Specified String pieces
    /// This depends on the Type on property
    /// </summary>
    /// <param name="type">Type of property</param>
    /// <param name="text">Caption/Display Description</param>
    /// <returns></returns>
    private static string ChangeCaption(Type type, string text)
    {
      if (type == typeof(DateTime))
      {
        return text.Replace("DT", "On");
      }
      return text;
    }

    /// <summary>
    /// Get the non-nullable Type of nullable Type
    /// </summary>
    /// <param name="type">Type to check</param>
    /// <returns>Non-Nullable Type</returns>
    private static Type GetNonNullableType(Type type)
    {
      if (type == typeof(DateTime?))
      {
        return typeof(DateTime);
      }
      else
      {
        return type;
      }
    }

    /// <summary>
    /// Get the Nullable Value for a Nullable Type
    /// </summary>
    /// <param name="type">Type of the object</param>
    /// <param name="value">value of object</param>
    /// <returns>returns the nullable Value</returns>
    private static object GetNullableValue(Type type, object value)
    {
      if (type == typeof(DateTime?))
      {
        return (value ?? DBNull.Value);
      }
      return value;
    }

    #endregion
  }
}
