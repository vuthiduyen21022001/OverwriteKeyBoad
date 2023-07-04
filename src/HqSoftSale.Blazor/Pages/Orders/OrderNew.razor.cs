using Blazorise;
using Blazorise.Bootstrap5;
using HqSoftSale.OrderDetails;
using HqSoftSale.Orders;
using HqSoftSale.Products;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.Users;

namespace HqSoftSale.Blazor.Pages.Orders
{
    public partial class OrderNew
    {
        private IReadOnlyList<OrderDto> orders { get; set; }
    //    private IReadOnlyList<OrdDetailDto> orderDetails { get; set; }
    private IReadOnlyList<ProductDto> products { get; set; }

        protected Validations CreateValationRef;
        protected CreateUpdateOrderDto NewEntity = new();
        protected CreateUpdateOrdDetailsDto NewDetailEntity = new();
        private int PageSize { get; set; } = 1000;
        private int CurrentPage { get; set; }
        private string CurrentSorting { get; set; }



        // code s 
        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    if (firstRender)
        //    {
        //        await JSRuntime.InvokeVoidAsync("addEventListener", "keydown", EventCallback.Factory.Create(this, HandleKeyDown));
        //    }
        //}

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await JSRuntime.InvokeVoidAsync("addEventListener", "keydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDown));            //document.AddEventListener("keydown", HandleKeyDown);
           
            if (CreateValationRef != null)
            {
                await CreateValationRef.ClearAll();
            }

            await CalculatePrice();
            await GetProductAsync();
        }


        ///code ctrl s 
        private async Task HandleSaveButtonClick()
        {
            // Perform save operation
            await JSRuntime.InvokeVoidAsync("alert", "Save operation completed successfully.");
        }

        private async Task HandleKeyDown(KeyboardEventArgs e)
        {
            // Check if the Ctrl key and S key were both pressed
            if (e.CtrlKey && e.Key == "s")
            {
                // Prevent the default behavior
                await JSRuntime.InvokeVoidAsync("preventDefault", e);


                // Trigger the save operation
                await HandleSaveButtonClick();
            }
        }





    

        ////////////////////






        public partial class Index
{
   public List<string> Logs { get; set; } = new();
   public void KeyEvent(KeyboardEventArgs e)
   {
      Logs.Clear();
      Logs.Add(e.Key);
      Logs.Add(e.Code);
      Logs.Add(e.Type);
   }
}








        /// code chặn trình duyệt
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("eval", @"
        window.addEventListener('keydown', function (event) {
          if (event.ctrlKey && event.key === 's') {
            event.preventDefault(); // prevent default browser behavior (e.g. save page)
            DotNet.invokeMethodAsync('YourAssemblyName', 'OnCtrlS'); // invoke Blazor method
          }
        });
      ");
            }
        }

        [JSInvokable]
        public async Task OnCtrlS()
        {
            // Handle Ctrl + S keyboard shortcut here...
        }






        ///code ctrl n , ctrl o <summary>
        /// code ctrl n , ctrl o

        /// </summary>
        /// <returns></returns>
     
    //private void HandleButtonClick()
    //    {
    //        // Handle button click event
    //    }
    //    private ElementReference textBoxRef;

    //    private async Task PreventDefaultAsync()
    //    {
    //        await JSRuntime.InvokeVoidAsync("preventDefault");
    //    }
    //    private async Task HandleTextBoxKeyDown(KeyboardEventArgs e)
    //    {
    //        if (e.CtrlKey && e.Key == "s")
    //        {
    //            // Override Ctrl+S shortcut for text box
    //            e.PreventDefault();
    //            await PreventDefaultAsync();
    //            // Perform desired action for text box
    //        }
    //        else if (e.CtrlKey && e.Key == "0")
    //        {
    //            // Override Ctrl+0 shortcut for text box
    //            e.PreventDefault();
    //            await PreventDefaultAsync();
    //            // Perform desired action for text box
    //        }
    //    }

    //    private async Task HandleNavigationKeyDown(KeyboardEventArgs e)
    //    {
    //        if (e.CtrlKey && e.Key == "n")
    //        {
    //            // Override Ctrl+N shortcut for navigation
    //            e.PreventDefault();
    //            await PreventDefaultAsync();
    //            // Perform desired action for navigation
    //        }
    //    }






        protected virtual async Task CreateEntityAsync()
        {
            try
            {
                var validate = true;
                if (CreateValationRef != null)
                {
                    validate = await CreateValationRef.ValidateAll();
                }
                if (validate)
                {
                    OrderAppService.CreateAsync(NewEntity);
                    OrderDetailAppService.CreateAsync(NewDetailEntity);                  
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task GetProductAsync()
        {
            var result = await ProductAppService.GetListAsync(
                new GetProductListDto
                {
                    MaxResultCount = PageSize,
                    SkipCount = CurrentPage * PageSize,
                    Sorting = CurrentSorting
                }
            );

            products = result.Items;
            TotalCount = (int)result.TotalCount;
        }



        //protected virtual async Task CreateProductEntityAsync()
        //{
        //    try
        //    {
        //        var validate = true;
        //        if (CreateValationRef != null)
        //        {
        //            validate = await CreateValationRef.ValidateAll();
        //        }
        //        if (validate)
        //        {
        //            await ProductAppService.CreateAsync(NewEntityProduct);
        //            //NavigationManager.NavigateTo("products");
        //            NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await HandleErrorAsync(ex);
        //    }
        //}

        private void UpdateTotal(int quantity)
        {
            NewDetailEntity.Quantity = quantity;
            NewDetailEntity.ExtenedAmount = NewDetailEntity.Quantity * NewDetailEntity.Price;
        }

        protected virtual async Task CalculatePrice()
        {
            if (!string.IsNullOrEmpty(NewDetailEntity.ProductID))
            {              
                var product = products.FirstOrDefault(p => p.ProductID == NewDetailEntity.ProductID);
                if (product != null)
                {                  
                    NewDetailEntity.Price = NewDetailEntity.Quantity * product.Price;
                }
            }
        }
    }
}

