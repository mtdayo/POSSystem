using Microsoft.AspNetCore.Components.Authorization;
using POSSystem.Infrastructure.Repositories;
using POSSystem.WebApp.Models;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace POSSystem.WebApp.Services
{
    public class CartService
    {
        private readonly CartRepository _cartRepository;
        private readonly ProductApiClient _productApiClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        private int? _accountid;
        private int? _cartId;
        private bool _isLoaded;


        public List<CartItem> Items { get; } = new();

        public string? ErrorMessage { get; private set; }

        public decimal TotalAmount => Items.Sum(item => item.Subtotal);

        public CartService(CartRepository cartrepository, ProductApiClient productApiClient, AuthenticationStateProvider authenticationStateProvider)
        {
            _cartRepository = cartrepository;
            _productApiClient = productApiClient;
            _authenticationStateProvider = authenticationStateProvider;
        }

        /// <summary>
        /// ログイン中のアカウントのカートをDBから読み取る
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task LoadCartAsync(bool forceReload = false)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

            var user = authState.User;

            if(user.Identity?.IsAuthenticated != true)
            {
                Items.Clear();
                _accountid = null;
                _cartId = null;
                _isLoaded = false;
                return;
            }

            var accountIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(!int.TryParse(accountIdValue, out int accountId))
            {
                throw new InvalidOperationException("ログイン中のアカウントIDを取得できません。");
            }

            //同じアカウントのカートを読み込み済みなら再取得しない
            if(!forceReload && _isLoaded && _accountid == accountId)
            {
                return;
            }

            Items.Clear();

            _accountid = accountId;
            _cartId = null;
            _isLoaded = false;

            var cart = _cartRepository.GetCartByAccountId(accountId);
            _cartId = cart.Id;

            var savedItems = _cartRepository.GetItemsByCartId(cart.Id);

            foreach (var savedItem in savedItems)
            {
                var product = await _productApiClient.GetProductByCodeAsync(savedItem.ProductCode);

                //商品が削除されている場合は画面に表示しない
                if (product == null)
                {
                    continue;
                }

                Items.Add(new CartItem
                {
                    CartItemId = savedItem.Id,
                    Product = product,
                    Quantity = savedItem.Quantity
                });
            }

            _isLoaded = true;

        }

        /// <summary>
        /// 商品をカートに追加できるか判断するメソッド
        /// </summary>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public bool AddProduct(Product product, int quantity)
        {
            ErrorMessage = null;


            if(!EnsureCartLoaded())
            {
                return false;
            }


            if(quantity <= 0)
            {
                ErrorMessage = "購入個数は１個以上にしてください。";
                return false;
            }

            //商品をitemに保存
            var item = Items.FirstOrDefault(
                x => x.Product.Code == product.Code);

            int currentQuantity = item?.Quantity ?? 0;

            //カートに入れた個数が在庫を超えた場合のエラーメッセージ
            if(currentQuantity + quantity > product.Stock)
            {
                ErrorMessage = $"{product.Name}の在庫が足りません。在庫数 : {product.Stock}個";
                return false;
            }

            if(item == null)
            {
                //先にDBへ商品を追加する
                _cartRepository.AddItem(_cartId!.Value, product.Code, quantity);

                //DBから追加後の商品を取得する
                var savedItems = _cartRepository.GetItemsByCartId(_cartId.Value);


                var savedItem = _cartRepository.GetItemsByCartId(_cartId.Value)
                                               .First(x => x.ProductCode == product.Code);

                if(savedItem == null)
                {
                    ErrorMessage = "カートへの商品追加に失敗しました。";
                    return false;
                }

                Items.Add(new CartItem
                {
                    CartItemId = savedItem.Id,
                    Product = product,
                    Quantity = savedItem.Quantity
                });
            }
            else
            {
                _cartRepository.AddItem(_cartId!.Value, product.Code, quantity);
                item.Quantity += quantity;
            }

            return true;
        }

        /// <summary>
        /// 追加した商品を更新できるか判断するメソッド
        /// </summary>
        /// <param name="item"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public bool UpdateQuantity(CartItem item, int quantity)
        {
            ErrorMessage = null;

            if(!EnsureCartLoaded())
            {
                return false;
            }

            if(quantity <= 0)
            {
                ErrorMessage = "購入個数は１個以上にして下さい。";
                return false;
            }

            if(quantity > item.Product.Stock)
            {
                ErrorMessage = $"{item.Product.Name}の在庫が足りません。在庫数 : {item.Product.Stock}個";
                return false;
            }

            bool updated = _cartRepository.UpdateQuantity(item.CartItemId, quantity);

            if(!updated)
            {
                ErrorMessage = "カートの数量を更新できませんでした。";
                return false;
            }

            item.Quantity = quantity;
            return true;
        }

        /// <summary>
        /// 商品をカートから削除するメソッド
        /// </summary>
        /// <param name="item"></param>
        public void RemoveProduct(CartItem item)
        {
            ErrorMessage = null;

            if(!EnsureCartLoaded())
            {
                return;
            }

            if(_cartRepository.RemoveItem(item.CartItemId))
            {
                Items.Remove(item);
            }
        }

        /// <summary>
        /// カートの中身をリセットするメソッド
        /// </summary>
        public void Clear()
        {
            ErrorMessage = null;

            if(!EnsureCartLoaded())
            {
                return;
            }

            _cartRepository.ClearItems(_cartId!.Value);
            Items.Clear();
        }

        private bool EnsureCartLoaded()
        {
            if(_isLoaded && _accountid.HasValue && _cartId.HasValue)
            {
                return true;
            }

            ErrorMessage = "カートを読み込めていません。ページを再読み込みしてください。";
            return false;
        }
    }
}
