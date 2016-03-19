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
        Bitmap bitmap;
        Canvas canvas;

        RectF circleOuterBounds, circleInnerBounds;

        Paint circlePaint, eraserPaint;
        Context context;
        float currentRating;
        float rating;

        ValueAnimator timerAnimator;

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
            get { return rating; }
            set
            {
                rating = value;
                currentRating = 0;
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
            this.context = context;
            circlePaint = new Paint
            {
                Color = new Color(ContextCompat.GetColor(context, Resource.Color.accent)),
                AntiAlias = true
            };

            eraserPaint = new Paint
            {
                AntiAlias = true,
                Color = Color.Transparent,
            };
            eraserPaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Clear));
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            if (w != oldw || h != oldh)
            {
                bitmap = Bitmap.CreateBitmap(w, h, Bitmap.Config.Argb8888);
                bitmap.EraseColor(Color.Transparent);
                canvas = new Canvas(bitmap);
            }
            base.OnSizeChanged(w, h, oldw, oldh);
            UpdateBounds();
        }

        void UpdateBounds()
        {
            var logicalDensity = context.Resources.DisplayMetrics.Density;
            var thickness = (int) Math.Ceiling(4*logicalDensity + .5f);

            circleOuterBounds = new RectF(0, 0, Width, Height);
            circleInnerBounds = new RectF(
                circleOuterBounds.Left + thickness,
                circleOuterBounds.Top + thickness,
                circleOuterBounds.Right - thickness,
                circleOuterBounds.Bottom - thickness);

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
            this.canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);


            var sweepAngle = PlayAnimation ? (currentRating/100f)*360 : (Rating/100f)*360;
            if (sweepAngle > 0f)
            {
                this.canvas.DrawArc(circleOuterBounds, 270, sweepAngle, true, circlePaint);
                this.canvas.DrawOval(circleInnerBounds, eraserPaint);
            }


            canvas.DrawBitmap(bitmap, 0, 0, null);
        }

        void Start(long secs)
        {
            timerAnimator = ValueAnimator.OfFloat(0f, Rating);
            timerAnimator.SetDuration(Java.Util.Concurrent.TimeUnit.Seconds.ToMillis(secs));
            timerAnimator.SetInterpolator(new AccelerateInterpolator());
            timerAnimator.Update += (sender, e) =>
            {
                currentRating = (float) e.Animation.AnimatedValue;
                Invalidate();
            };
            timerAnimator.Start();
        }
    }
}