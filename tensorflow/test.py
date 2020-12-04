import numpy
import matplotlib.pyplot as plt
from pandas import read_csv
import math
import random
import sys, getopt
from keras.models import Sequential
from keras.layers import Dense
from keras.layers import Dropout
from keras.layers import LSTM
from sklearn.preprocessing import MinMaxScaler
from sklearn.metrics import mean_squared_error
#
# learn.py: learn the whole database and save the model
# test.py: test the model with the learned database
#	- use some params to predict not only one element in the future
# predict.py: predict the future using the saved model and a database
#
# convert an array of values into a dataset matrix
def create_dataset(dataset, look_back=1):
	dataX, dataY = [], []
	for i in range(len(dataset)-look_back-1):
		a = dataset[i:(i+look_back), 0]
		dataX.append(a)
		dataY.append(dataset[i + look_back, 0])
	return numpy.array(dataX), numpy.array(dataY)

def main(argv):
	inputfile = ''
	outputfile = ''
	try:
		opts, args = getopt.getopt(argv,"hi:",["ifile="])
	except getopt.GetoptError:
        sys.exit(2)

	for opt, arg in opts:
		if opt == '-h':
			print 'test.py -i <inputfile>'
			sys.exit()
        elif opt in ("-i", "--ifile"):
            inputfile = arg
	print 'Input file is "', inputfile

inputModel = "";
if __name__ == "__main__":
    inputModel = main(sys.argv[1:])

# fix random seed for reproducibility
numpy.random.seed(7)

# load the dataset
url = "/Users/gbodacs/Projects/WatcherO/tensorflow/IBM.csv"
dataframe = read_csv(url, usecols=[4])
train_size = 0
test_size = len(dataframe)
predictLen = 25
epochNumber = 100

dataset = dataframe.values
dataset = dataset[0:train_size+test_size+predictLen]
dataset = dataset.astype('float32')

# normalize the dataset
scaler = MinMaxScaler(feature_range=(0, 1))
dataset = scaler.fit_transform(dataset)
# load test sets

test = dataset[0:test_size,:]

# reshape into X=t and Y=t+1
look_back = 1
testX, testY = create_dataset(test, look_back)

# reshape input to be [samples, time steps, features]
testX = numpy.reshape(testX, (testX.shape[0], 1, testX.shape[1]))

# load model
model = keras.models.load_model(inputfile)

# make predictions
testPredict = model.predict(testX)
for x in range(predictLen):
    temp = testPredict[len(testPredict)-1]
    print('Added predict: %.2f value' % (temp))
    test = numpy.append(test, [temp], axis=0 )
    testX, testY = create_dataset(test, look_back)
    testX = numpy.reshape(testX, (testX.shape[0], 1, testX.shape[1]))
    testPredict = model.predict(testX)

# invert predictions
testPredict = scaler.inverse_transform(testPredict)
testY = scaler.inverse_transform([testY])

# calculate root mean squared error
testScore = math.sqrt(mean_squared_error(testY[0], testPredict[:,0]))
print('Test Score: %.2f RMSE' % (testScore))

truePredict = testPredict[-predictLen:]
testPredict = testPredict[0:-predictLen]

# shift test predictions for plotting
testPredictPlot = numpy.empty_like(dataset)
testPredictPlot[:, :] = numpy.nan
testPredictPlot[look_back*2+1:len(testPredictPlot)-1-predictLen, :] = testPredict

truePredictPlot = numpy.empty_like(dataset)
truePredictPlot[:, :] = numpy.nan
truePredictPlot[-predictLen-1:len(testPredictPlot)-1, :] = truePredict

# plot baseline and predictions
plt.plot(scaler.inverse_transform(dataset))
plt.plot(testPredictPlot)
plt.plot(truePredictPlot)
plt.show()