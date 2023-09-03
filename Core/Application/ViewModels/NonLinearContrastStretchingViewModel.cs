using System;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using ReactiveUI;
using Splat;

namespace ImageManipulator.Application.ViewModels;

public class NonLinearContrastStretchingViewModel : ImageOperationDialogViewModelBase
{
	private readonly IImagePointOperationsService imagePointOperationsService;
	private Avalonia.Media.Imaging.Bitmap _beforeImage;
	private Avalonia.Media.Imaging.Bitmap _afterImage;
	private double _gamma;

	public override Avalonia.Media.Imaging.Bitmap BeforeImage { get => _beforeImage; set
		{
			_ = this.RaiseAndSetIfChanged(ref _beforeImage, value);
		}
	}

	public override Avalonia.Media.Imaging.Bitmap AfterImage
	{
		get => _afterImage; set
		{
			this.RaiseAndSetIfChanged(ref _afterImage, value);
		}
	}

	public double GammaValue { get => _gamma; set => this.RaiseAndSetIfChanged(ref _gamma, value); }

	#region Commands 
	public ReactiveCommand<Unit, Unit> ExecuteNonLinearStretching { get; }

	#endregion

	public NonLinearContrastStretchingViewModel(IImagePointOperationsService imagePointOperationsService)
	{
		this.imagePointOperationsService = imagePointOperationsService;
		ExecuteNonLinearStretching = ReactiveCommand.CreateFromObservable(NonLinearlyStretchContrast);
		ExecuteNonLinearStretching.IsExecuting.ToProperty(this, x => x.IsExecuting, out _isExecuting);
		ExecuteNonLinearStretching.ThrownExceptions.Subscribe(ex =>
			this.Log().ErrorException("Error during stretching!", ex));
		AcceptCommand = new RelayCommand<Window>(this.Accept, x => AcceptCommandCanExecute());
		CancelCommand = new RelayCommand<Window>(this.Cancel);		}

	private IObservable<Unit> NonLinearlyStretchContrast() =>
		Observable.Start(() =>
		{
			var stretchedImage =
				imagePointOperationsService.NonLinearlyStretchContrast(
					ImageConverterHelper.ConvertFromAvaloniaUIBitmap(_beforeImage), _gamma);
			AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(stretchedImage);
		});
}