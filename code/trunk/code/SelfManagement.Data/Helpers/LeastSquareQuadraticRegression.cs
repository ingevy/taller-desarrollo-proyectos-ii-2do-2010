namespace CallCenter.SelfManagement.Data.Helpers
{
    using System;
    using System.Collections;

    public class LeastSquareQuadraticRegression
    {
        private ArrayList pointArray = new ArrayList();
        private int numOfEntries;
        private double[] pointpair;

        public LeastSquareQuadraticRegression()
        {
            this.numOfEntries = 0;
            this.pointpair = new double[2];
        }

        /// <summary>
        /// add point pairs
        /// </summary>
        /// <param name="x">x value</param>
        /// <param name="y">y value</param>
        public void AddPoints(double x, double y)
        {
            this.pointpair = new double[2];

            this.numOfEntries += 1;
            this.pointpair[0] = x;
            this.pointpair[1] = y;

            this.pointArray.Add(this.pointpair);
        }

        /// <summary>
        /// returns the a term of the equation ax^2 + bx + c
        /// </summary>
        /// <returns>a term</returns>
        public double ATerm()
        {
            if (this.numOfEntries < 3)
            {
                throw new InvalidOperationException("Insufficient pairs of co-ordinates");
            }

            //notation sjk to mean the sum of x_i^j*y_i^k. 
            double s40 = GetSx4(); //sum of x^4
            double s30 = GetSx3(); //sum of x^3
            double s20 = GetSx2(); //sum of x^2
            double s10 = GetSx();  //sum of x
            double s00 = this.numOfEntries;
            
            //sum of x^0 * y^0  ie 1 * number of entries

            double s21 = GetSx2y(); //sum of x^2*y
            double s11 = GetSxy();  //sum of x*y
            double s01 = GetSy();   //sum of y

            //a = Da/D
            return (s21 * (s20 * s00 - s10 * s10) -
                    s11 * (s30 * s00 - s10 * s20) +
                    s01 * (s30 * s10 - s20 * s20))
                    /
                    (s40 * (s20 * s00 - s10 * s10) -
                     s30 * (s30 * s00 - s10 * s20) +
                     s20 * (s30 * s10 - s20 * s20));
        }

        /// <summary>
        /// returns the b term of the equation ax^2 + bx + c
        /// </summary>
        /// <returns>b term</returns>
        public double BTerm()
        {
            if (this.numOfEntries < 3)
            {
                throw new InvalidOperationException("Insufficient pairs of co-ordinates");
            }

            //notation sjk to mean the sum of x_i^j*y_i^k.
            double s40 = GetSx4(); //sum of x^4
            double s30 = GetSx3(); //sum of x^3
            double s20 = GetSx2(); //sum of x^2
            double s10 = GetSx();  //sum of x
            double s00 = this.numOfEntries;
            
            //sum of x^0 * y^0  ie 1 * number of entries

            double s21 = GetSx2y(); //sum of x^2*y
            double s11 = GetSxy();  //sum of x*y
            double s01 = GetSy();   //sum of y

            //b = Db/D
            return (s40 * (s11 * s00 - s01 * s10) -
                    s30 * (s21 * s00 - s01 * s20) +
                    s20 * (s21 * s10 - s11 * s20))
                    /
                    (s40 * (s20 * s00 - s10 * s10) -
                     s30 * (s30 * s00 - s10 * s20) +
                     s20 * (s30 * s10 - s20 * s20));
        }

        /// <summary>
        /// Returns the c term of the equation ax^2 + bx + c
        /// </summary>
        /// <returns>c term</returns>
        public double CTerm()
        {
            if (this.numOfEntries < 3)
            {
                throw new InvalidOperationException("Insufficient pairs of co-ordinates");
            }

            //notation sjk to mean the sum of x_i^j*y_i^k.
            double s40 = GetSx4(); //sum of x^4
            double s30 = GetSx3(); //sum of x^3
            double s20 = GetSx2(); //sum of x^2
            double s10 = GetSx();  //sum of x
            double s00 = this.numOfEntries;
            
            //sum of x^0 * y^0  ie 1 * number of entries

            double s21 = GetSx2y(); //sum of x^2*y
            double s11 = GetSxy();  //sum of x*y
            double s01 = GetSy();   //sum of y

            //c = Dc/D
            return (s40 * (s20 * s01 - s10 * s11) -
                    s30 * (s30 * s01 - s10 * s21) +
                    s20 * (s30 * s11 - s20 * s21))
                    /
                    (s40 * (s20 * s00 - s10 * s10) -
                     s30 * (s30 * s00 - s10 * s20) +
                     s20 * (s30 * s10 - s20 * s20));
        }

        // get r-squared
        public double RSquare() 
        {
            if (this.numOfEntries < 3)
            {
                throw new InvalidOperationException("Insufficient pairs of co-ordinates");
            }

            // 1 - (residual sum of squares / total sum of squares)
            return 1 - GetSSerr() / GetSStot();
        }

        // get sum of x
        private double GetSx()
        {
            double Sx = 0;
            foreach (double[] ppair in this.pointArray)
            {
                Sx += ppair[0];
            }

            return Sx;
        }

        // get sum of y
        private double GetSy()
        {
            double sy = 0;
            foreach (double[] ppair in this.pointArray)
            {
                sy += ppair[1];
            }

            return sy;
        }

        // get sum of x^2
        private double GetSx2()
        {
            double sx2 = 0;
            foreach (double[] ppair in this.pointArray)
            {
                sx2 += Math.Pow(ppair[0], 2); // sum of x^2
            }

            return sx2;
        }

        // get sum of x^3
        private double GetSx3()
        {
            double sx3 = 0;
            foreach (double[] ppair in this.pointArray)
            {
                sx3 += Math.Pow(ppair[0], 3); // sum of x^3
            }

            return sx3;
        }

        // get sum of x^4
        private double GetSx4()
        {
            double sx4 = 0;
            foreach (double[] ppair in this.pointArray)
            {
                sx4 += Math.Pow(ppair[0], 4); // sum of x^4
            }

            return sx4;
        }

        // get sum of x*y
        private double GetSxy()
        {
            double sxy = 0;
            foreach (double[] ppair in this.pointArray)
            {
                sxy += ppair[0] * ppair[1]; // sum of x*y
            }

            return sxy;
        }

        // get sum of x^2*y
        private double GetSx2y()
        {
            double sx2y = 0;
            foreach (double[] ppair in this.pointArray)
            {
                sx2y += Math.Pow(ppair[0], 2) * ppair[1]; // sum of x^2*y
            }

            return sx2y;
        }

        // mean value of y
        private double GetYMean()
        {
            double y_tot = 0;
            foreach (double[] ppair in this.pointArray)
            {
                y_tot += ppair[1];
            }

            return y_tot / this.numOfEntries;
        }

        private double GetSStot() // total sum of squares
        {
            //the sum of the squares of the differences between 
            //the measured y values and the mean y value
            double ss_tot = 0;
            foreach (double[] ppair in this.pointArray)
            {
                ss_tot += Math.Pow(ppair[1] - GetYMean(), 2);
            }

            return ss_tot;
        }

        // residual sum of squares
        private double GetSSerr()
        {
            //the sum of the squares of te difference between 
            //the measured y values and the values of y predicted by the equation
            double ss_err = 0;
            foreach (double[] ppair in this.pointArray)
            {
                ss_err += Math.Pow(ppair[1] - GetPredictedY(ppair[0]), 2);
            }

            return ss_err;
        }

        private double GetPredictedY(double x)
        {
            // returns value of y predicted by the equation for a given value of x
            return ATerm() * Math.Pow(x, 2) + BTerm() * x + CTerm();
        }

        // Retorna valor Y, para un X pedido
        public double CalculatePredictedY(double x)
        {
            return GetPredictedY(x);
        }
    }
}
