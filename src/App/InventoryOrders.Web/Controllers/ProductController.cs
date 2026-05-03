using InventoryOrders.Application.Filters;
using InventoryOrders.Application.Logging;
using InventoryOrders.Application.Repositories;
using InventoryOrders.Application.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InventoryOrders.Web.Controllers;

public class ProductController(IProductRepository productRepository, IAppLogger<ProductController> logger) : Controller
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IAppLogger<ProductController> _logger = logger;

    public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var filter = new Filter
            {
                Search = search,
                IsDelete = false,
                Page = page,
                PageSize = pageSize
            };
            _logger.LogInfo($"Fetching products. Search={search}, Page={page}, PageSize={pageSize}");
            var pagination = await _productRepository.GetProductsAsync(filter, cancellationToken);
            _logger.LogInfo($"Fetched {pagination.Items.Count()} products");
            return View(pagination);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while fetching products", ex);
            return StatusCode(500, "An error occurred while fetching products.");
        }
    }

    [HttpGet]
    [Route("product/createoredit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(long id = 0, CancellationToken cancellationToken = default)
    {
        try
        {
            ProductVm productVm = new();

            if (id > 0)
            {
                productVm = await _productRepository.GetProductByIdAsync(id, cancellationToken);
                if (productVm == null)
                {
                    TempData["AlertMessage"] = $"Product with Id {id} not found.";
                    TempData["AlertType"] = "Error";
                    return NotFound();
                }
            }

            return View(productVm);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in CreateOrEdit for Id={id}", ex);
            return StatusCode(500, "An error occurred while opening the form.");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("product/createoredit/{id?}")]
    public async Task<IActionResult> CreateOrEdit(ProductVm productVm, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertMessage"] = "Please fix validation errors.";
            TempData["AlertType"] = "Warning";
            return View(productVm);
        }
        try
        {
            var result = await _productRepository.CreateOrUpdateProductAsync(productVm, cancellationToken);

            if (result == null)
            {
                TempData["AlertMessage"] = $"Product with Id {productVm.Id} not found.";
                TempData["AlertType"] = "Error";
                return NotFound();
            }

            TempData["AlertMessage"] = productVm.Id > 0
                ? "Product updated successfully!"
                : "Product created successfully!";
            TempData["AlertType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while creating/updating product", ex);
            TempData["AlertMessage"] = "An error occurred while saving the product.";
            TempData["AlertType"] = "Error";
            return StatusCode(500, "An error occurred while saving the product.");
        }
    }

    [HttpPost]
    [Route("product/delete/{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var deleted = await _productRepository.DeleteProductAsync(id, cancellationToken);

            if (!deleted)
            {
                TempData["AlertMessage"] = $"Product with Id {id} not found.";
                TempData["AlertType"] = "Error";
                return NotFound();
            }

            TempData["AlertMessage"] = "Product deleted successfully!";
            TempData["AlertType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while deleting product Id={id}", ex);
            TempData["AlertMessage"] = "An error occurred while deleting the product.";
            TempData["AlertType"] = "Error";
            return StatusCode(500, "An error occurred while deleting the product.");
        }
    }
}