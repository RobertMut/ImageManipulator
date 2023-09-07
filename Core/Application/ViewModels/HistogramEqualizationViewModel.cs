using System;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ReactiveUI;
using Splat;

namespace ImageManipulator.Application.ViewModels;

public class HistogramEqualizationViewModel : ImageOperationDialogViewModelBase
{
	private readonly IImagePointOperationsService _imagePointOperationsService;
	private Bitmap? _beforeImage;
	private Bitmap? _afterImage;
	private double _gamma;
	public int[]?[] lut;

	public override Bitmap? BeforeImage { get => _beforeImage; set
		{
			_ = this.RaiseAndSetIfChanged(ref _beforeImage, value);
		}
	}

	public override Bitmap? AfterImage
	{
		get => _afterImage; set
		{
			this.RaiseAndSetIfChanged(ref _afterImage, value);
		}
	}

	public double GammaValue { get => _gamma; set => this.RaiseAndSetIfChanged(ref _gamma, value); }

	#region Commands 
	public ReactiveCommand<Unit, Unit> ExecuteEqualizeHistogram { get; }

	#endregion

	public HistogramEqualizationViewModel(IImagePointOperationsService imagePointOperationsService)
	{
		_imagePointOperationsService = imagePointOperationsService;
		ExecuteEqualizeHistogram = ReactiveCommand.CreateFromObservable(Equalization);
		ExecuteEqualizeHistogram.IsExecuting.ToProperty(this, x => x.IsExecuting, out _isExecuting);
		ExecuteEqualizeHistogram.ThrownExceptions.Subscribe(ex =>
			this.Log().ErrorException("Error during equalization!", ex));
		AcceptCommand = new RelayCommand<Window>(this.Accept, x => AcceptCommandCanExecute());
		CancelCommand = new RelayCommand<Window>(this.Cancel);
	}
		
	private IObservable<Unit> Equalization() =>
		Observable.Start(() =>
		{
			var equalizedImage =
				_imagePointOperationsService.HistogramEqualization(
					ImageConverterHelper.ConvertFromAvaloniaUIBitmap(BeforeImage), lut);
			AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(equalizedImage);
		});
}