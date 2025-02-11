using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OnThisDay.Models;

namespace OnThisDay.Controllers
{
	[Route("[controller]")]
	public class OTDController : Controller
	{
		private readonly HttpClient _httpClient;
		
		public OTDController()
		{
			_httpClient = new HttpClient();
		}

	public async Task<IActionResult> Index(string day = null, string month = null)
	{
		if(string.IsNullOrEmpty(month) || string.IsNullOrEmpty(day))
		{
			month = DateTime.Now.ToString("MM");
			day = DateTime.Now.ToString("dd");
			
		}
		try
		{
			int monthNum = int.Parse(month);
			string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNum);
			ViewData["month"] = monthName;
			ViewData["day"] = day;
			
			var url = $"https://api.wikimedia.org/feed/v1/wikipedia/en/onthisday/all/{month}/{day}";
			HttpResponseMessage response = await _httpClient.GetAsync(url);

			// Log status code and response content
			string responseBody = await response.Content.ReadAsStringAsync();
			Console.WriteLine($"API Response Status: {response.StatusCode}");
			Console.WriteLine($"API Response Body: {responseBody}");

			// Ensure it's a successful response
			response.EnsureSuccessStatusCode();

			var contents = JsonConvert.DeserializeObject<OTD.Root>(responseBody);
			return View(contents);
		}
		catch (HttpRequestException ex)
		{
			Console.WriteLine($"Request failed: {ex.Message}");
			return StatusCode(500, "Error fetching data from Wikimedia API! \n Please refresh the page to retry");
			
		}
	}
	}
}