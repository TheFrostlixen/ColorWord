/*
 * ColorWord
 * Author: Matt Fredrickson
 * Date: 5/21/2015
 */
using System;

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
			Console.Write("Enter a word to detect its equivalent color: ");
			string word = Console.ReadLine();
			
			int[] colorList = new int[ word.Length ];
			
			for (int i = 0; i < word.Length; i++)
			{
				if (word[i] >= 'a' && word[i] <= 'z')
					colorList[i] = GetColorFromwavelength( getwavelength( word[i] ) );
			}
			
			Console.WriteLine( "Average: " + averageColors( colorList ).ToString("X") );
			Console.WriteLine( "Square Root: " + squaredColors( colorList ).ToString("X") );
			
			Console.ReadKey();
		}
		
		public static int averageColors( int[] list )
		{
			int total = 0;
			foreach (int i in list)
			{
				total += i;
			}
			return total / list.Length;
		}
		
		public static int squaredColors( int[] list )
		{
			long total = 0;
			foreach (int i in list)
			{
				total += (long)Math.Pow( i, 2 );
			}
			return (int)Math.Floor( Math.Sqrt( total ) );
		}
		
		public static double getwavelength( char letter )
		{
			return _ULTRAVIOLET + (double)Math.Floor( (letter - 'a') * (_INFRARED - _ULTRAVIOLET) / 26.0 );
		}
		
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
			//return Color.FromArgb( 0, rgb[0], rgb[1], rgb[2] ).ToArgb();
		}
		
	}
}
