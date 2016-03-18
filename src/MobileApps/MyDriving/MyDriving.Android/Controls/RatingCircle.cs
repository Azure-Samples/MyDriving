// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Animation;
using System;
using Android.Support.V4.Content;

namespace MyDriving.Droid.Controls
{
    public class RatingCircle : View
    {
        Bitmap _bitmap;
        Canvas _canvas;

        RectF _circleOuterBounds, _circleInnerBounds;

        Paint _circlePaint, _eraserPaint;
        Context _context;
        float _currentRating;
        float _rating;

        ValueAnimator _timerAnimator;

        public RatingCircle(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {
        }

        public RatingCircle(Context context)
            : this(context, null)
        {
        }

        public RatingCircle(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Init(context, attrs);
        }

        public RatingCircle(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            Init(context, attrs);
        }

        public float Rating
        {
            get { return _rating; }
            set
            {
                _rating = value;
                _currentRating = 0;
                if (PlayAnimation)
                    Start(1);
                else
                    Invalidate();
            }
        }

        public bool PlayAnimation { get; set; } = true;

        void Init(Context context, IAttributeSet attributeSet)
        {
            SetBackgroundColor(Color.Transparent);
            _context = context;
            _circlePaint = new Paint
            {
                Color = new Color(ContextCompat.GetColor(context, Resource.Color.accent)),
                AntiAlias = true
            };

            _eraserPaint = new Paint
            {
                AntiAlias = true,
                Color = Color.Transparent,
            };
            _eraserPaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Clear));
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            if (w != oldw || h != oldh)
            {
                _bitmap = Bitmap.CreateBitmap(w, h, Bitmap.Config.Argb8888);
                _bitmap.EraseColor(Color.Transparent);
                _canvas = new Canvas(_bitmap);
            }
            base.OnSizeChanged(w, h, oldw, oldh);
            UpdateBounds();
        }

        void UpdateBounds()
        {
            var logicalDensity = _context.Resources.DisplayMetrics.Density;
            var thickness = (int) Math.Ceiling(4*logicalDensity + .5f);

            _circleOuterBounds = new RectF(0, 0, Width, Height);
            _circleInnerBounds = new RectF(
                _circleOuterBounds.Left + thickness,
                _circleOuterBounds.Top + thickness,
                _circleOuterBounds.Right - thickness,
                _circleOuterBounds.Bottom - thickness);

            Invalidate();
        }

        //make a perfect square
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var spec = widthMeasureSpec;
            base.OnMeasure(spec, spec);
        }

        protected override void OnDraw(Canvas canvas)
        {
            _canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);


            var sweepAngle = PlayAnimation ? (_currentRating/100f)*360 : (Rating/100f)*360;
            if (sweepAngle > 0f)
            {
                _canvas.DrawArc(_circleOuterBounds, 270, sweepAngle, true, _circlePaint);
                _canvas.DrawOval(_circleInnerBounds, _eraserPaint);
            }


            canvas.DrawBitmap(_bitmap, 0, 0, null);
        }

        void Start(long secs)
        {
            _timerAnimator = ValueAnimator.OfFloat(0f, Rating);
            _timerAnimator.SetDuration(Java.Util.Concurrent.TimeUnit.Seconds.ToMillis(secs));
            _timerAnimator.SetInterpolator(new AccelerateInterpolator());
            _timerAnimator.Update += (sender, e) =>
            {
                _currentRating = (float) e.Animation.AnimatedValue;
                Invalidate();
            };
            _timerAnimator.Start();
        }
    }
}