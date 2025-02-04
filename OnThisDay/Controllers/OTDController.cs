using System;
using System.Collections.Generic;
using System.Diagnostics;
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

	public async Task<IActionResult> Index()
	{
		try
		{
			var url = "https://api.wikimedia.org/feed/v1/wikipedia/en/onthisday/all/02/04";
			
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
			return StatusCode(500, "Error fetching data from Wikimedia API! /n Please refresh the browser to retry");
		}
	}
	}
}