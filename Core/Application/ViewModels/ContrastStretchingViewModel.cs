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

public class ContrastStretchingViewModel : ImageOperationDialogViewModelBase
{
	private readonly IImagePointOperationsService imagePointOperationsService;
	private readonly IImageDataService imageDataService;
	private Bitmap? _beforeImage;
	private Bitmap? _afterImage;
	private int _enteredLowerThreshold;
	private int _enteredUpperThreshold;

	public override Bitmap? BeforeImage { get => _beforeImage; set
		{
			CalculateSuggestedThresholds();
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

	public int[]? histogramValues { get; set; }
	public int LowerThreshold { get; set; }
	public int UpperThreshold { get; set; }
	public int EnteredLowerThreshold { get => _enteredLowerThreshold; set => this.RaiseAndSetIfChanged(ref _enteredLowerThreshold, value); }
	public int EnteredUpperThreshold { get => _enteredUpperThreshold; set => this.RaiseAndSetIfChanged(ref _enteredUpperThreshold, value); }
	#region Commands 
	public ReactiveCommand<Unit, Unit> ExecuteLinearStretching { get; }

	#endregion

	public ContrastStretchingViewModel(IImagePointOperationsService imagePointOperationsService)
	{
		this.imagePointOperationsService = imagePointOperationsService;
		ExecuteLinearStretching = ReactiveCommand.CreateFromObservable(LinearlyStreatchContrast);
		ExecuteLinearStretching.IsExecuting.ToProperty(this, x => x.IsCommandActive, out isCommandActive);
		ExecuteLinearStretching.ThrownExceptions.Subscribe(ex =>
			this.Log().ErrorException("Error during thresholding!", ex));
		AcceptCommand = new RelayCommand<Window>(this.Accept, x => AcceptCommandCanExecute());
		CancelCommand = new RelayCommand<Window>(this.Cancel);		}

	private void CalculateSuggestedThresholds()
	{
		LowerThreshold = imagePointOperationsService.CalculateLowerImageThresholdPoint(histogramValues);
		UpperThreshold = imagePointOperationsService.CalculateUpperImageThresholdPoint(histogramValues);
	}

	private IObservable<Unit> LinearlyStreatchContrast() =>
		Observable.Start(() =>
		{
			var stretchedImage = imagePointOperationsService.StretchContrast(
				ImageConverterHelper.ConvertFromAvaloniaUIBitmap(_beforeImage), _enteredLowerThreshold,
				_enteredUpperThreshold);
			AfterImage = ImageConverterHelper.ConvertFromSystemDrawingBitmap(stretchedImage);
		});
}