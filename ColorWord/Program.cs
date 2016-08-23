/*
 * ColorWord
 * Author: Matt Fredrickson
 * Date: 5/21/2015
 */
using System;
using System.IO;

namespace ColorWord
{
	class Program
	{
		private const int _ULTRAVIOLET = 380; // wavelength in nm
		private const int _INFRARED = 780; // wavelength in nm
		
		private const double Gamma = 0.80;
		private const double IntensityMax = 255;
		
		public static void Main(string[] args)
		{
			while (true)
			{
				// User input stage
				Console.Write("Enter a message to compute its equivalent color: ");
				string word = Console.ReadLine();
				
				// Begin color/letter processing
				int[] colorList = new int[ word.Length ];
				
				for (int i = 0; i < word.Length; i++)
				{
					if ( char.ToLower(word[i]) >= 'a' && char.ToLower(word[i]) <= 'z') // check if between a-z & adjust for caps
						colorList[i] = GetColorFromwavelength( getwavelength( char.ToLower(word[i]) ) );
				}
				
				// Compute and display results
				int avg = averageColors( colorList );
				int sqr = squaredColors( colorList );
				Console.WriteLine( "Average: " + avg.ToString("X") ); // format string as hex
				Console.WriteLine( "Square Root: " + sqr.ToString("X") );
				
				// Visually display in HTML
				Console.Write("Generate HTML? ");
				char y = (char)Console.ReadLine()[0];
				if (y == 'y' || y == 'Y')
					DisplayHtml( word.ToLower(), avg.ToString("X"), sqr.ToString("X") );
				
				// Wait for exit
				Console.WriteLine();
			}
		}
		
		/// <summary>
		/// Computes the 'average' of each letters calculated color for the message.
		/// </summary>
		/// <param name="list">list of all calculated colors</param>
		/// <returns>resultant message color</returns>
		public static int averageColors( int[] list )
		{
			int total = 0;
			foreach (int i in list)
			{
				total += i;
			}
			return total / list.Length;
		}
		
		/// <summary>
		/// Computes the 'square' of the colors calculated for the message.
		/// </summary>
		/// <param name="list">list of all calculated colors</param>
		/// <returns>resultant message color</returns>
		public static int squaredColors( int[] list )
		{
			long total = 0;
			foreach (int i in list)
			{
				total += (long)Math.Pow( i, 2 );
			}
			int result = (int)Math.Floor( Math.Sqrt(total) );
			return (result > 0x1000000) ? result % 0x1000000 : result; // color correction, prevents color from exceeding possible bounds
		}
		
		/// <summary>
		/// Calculates the corresponding wavelength (based on the visible spectrum) for a given letter.
		/// </summary>
		/// <returns>wavelength as a double</returns>
		public static double getwavelength( char letter )
		{
			return _ULTRAVIOLET + (double)Math.Floor( (letter - 'a') * (_INFRARED - _ULTRAVIOLET) / 26.0 );
		}
		
		/// <summary>
		/// Calculates an RGB color from a given wavelength of light.
		/// </summary>
		/// <param name="wavelength">Light wavelenght (in nm)</param>
		/// <returns>RGB color value</returns>
		public static int GetColorFromwavelength( double wavelength )
		{
			double factor;
			double Red, Green, Blue;

			if((wavelength >= 380) && (wavelength < 440))
			{
				Red = -(wavelength - 440) / (440 - 380);
				Green = 0.0;
				Blue = 1.0;
			}
			else if((wavelength >= 440) && (wavelength < 490))
			{
				Red = 0.0;
				Green = (wavelength - 440) / (490 - 440);
				Blue = 1.0;
			}
			else if((wavelength >= 490) && (wavelength < 510))
			{
				Red = 0.0;
				Green = 1.0;
				Blue = -(wavelength - 510) / (510 - 490);
			}
			else if((wavelength >= 510) && (wavelength < 580))
			{
				Red = (wavelength - 510) / (580 - 510);
				Green = 1.0;
				Blue = 0.0;
			}
			else if((wavelength >= 580) && (wavelength < 645))
			{
				Red = 1.0;
				Green = -(wavelength - 645) / (645 - 580);
				Blue = 0.0;
			}
			else if((wavelength >= 645) && (wavelength < 781))
			{
				Red = 1.0;
				Green = 0.0;
				Blue = 0.0;
			}
			else
			{
				Red = 0.0;
				Green = 0.0;
				Blue = 0.0;
			}

			// Let the intensity fall off near the vision limits
			if ( (wavelength >= 380) && (wavelength<420) )
				factor = 0.3 + 0.7 * (wavelength - 380) / (420 - 380);
			else if ( (wavelength >= 420) && (wavelength<701) )
				factor = 1.0;
			else if ( (wavelength >= 701) && (wavelength<781) )
				factor = 0.3 + 0.7 * (780 - wavelength) / (780 - 700);
			else
				factor = 0.0;

			// break down into rgb components
			int[] rgb = new int[3];
			rgb[0] = (Red == 0.0)	?	0	:	(int)Math.Round(IntensityMax * Math.Pow(Red * factor, Gamma));
			rgb[1] = (Green == 0.0)	?	0	:	(int)Math.Round(IntensityMax * Math.Pow(Green * factor, Gamma));
			rgb[2] = (Blue == 0.0)	?	0	:	(int)Math.Round(IntensityMax * Math.Pow(Blue * factor, Gamma));
			
			// combine into one int and return it
			return (0 << 24) | (rgb[0] << 16) | (rgb[1] << 8) | rgb[2];
		}
		
		/// <summary>
		/// Builds the simplest html page to display the averaged color
		/// </summary>
		/// <param name="msg">The </param>
		/// <param name="color1">RGB average color, formatted as hex.</param>
		/// <param name="color2">RGB squared color, formatted as hex.</param>
		public static void DisplayHtml( string msg, string color1, string color2 )
		{
			string filename = msg + ".html";
			string html = "<style type=\"text/css\">#top, #bottom {{ position: fixed; left: 0; right: 0; height: 50%;}}#top {{ top: 0; background-color:#{1};}}#bottom {{ bottom: 0; background-color:#{2};}}</style><div id=\"top\">{0}</div><div id=\"bottom\">{0}</div>";
			html = String.Format( html, msg, color1, color2 );
			File.WriteAllText( filename, html );
			System.Diagnostics.Process.Start( filename );
		}
	}
}
