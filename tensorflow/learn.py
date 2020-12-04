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
	try:
		opts, args = getopt.getopt(argv,"hi:o:",["ifile=","ofile="])
	except getopt.GetoptError:
    	print 'learn.py -i <inputfile>'
    	sys.exit(2)
	for opt, arg in opts:
		if opt == '-h':
			print 'learn.py -i <inputfile>'
			sys.exit()
      elif opt in ("-i", "--ifile"):
		  inputfile = arg
	print 'Input file is "', inputfile, '"'
	
if __name__ == "__main__":
    main(sys.argv[1:])

# fix random seed for reproducibility
numpy.random.seed(7)

# load the dataset
#url = """e:\Work\_tensorflow_model_scripts\corn-prices-historical-chart-data_2.csv"""
url = inputfile
dataframe = read_csv(url, usecols=[4])
train_size = len(dataframe) - 100
test_size = 99
predictLen = 25
epochNumber = 100

dataset = dataframe.values
dataset = dataset[0:train_size+test_size+predictLen]
dataset = dataset.astype('float32')

# normalize the dataset
scaler = MinMaxScaler(feature_range=(0, 1))
dataset = scaler.fit_transform(dataset)
# split into train and test sets

train, test = dataset[0:train_size,:], dataset[train_size:train_size+test_size,:]

# reshape into X=t and Y=t+1
look_back = 1
trainX, trainY = create_dataset(train, look_back)
testX, testY = create_dataset(test, look_back)

# reshape input to be [samples, time steps, features]
trainX = numpy.reshape(trainX, (trainX.shape[0], 1, trainX.shape[1]))
testX = numpy.reshape(testX, (testX.shape[0], 1, testX.shape[1]))

# load model
#model = keras.models.load_model('/Users/gbodacs/Projects/WatcherO/tensorflow/IBM_data3300_n68_lb1_day')

# create and fit the LSTM network
model = Sequential()
#model.add(LSTM(4, input_shape=(1, look_back)))

#Adding the first LSTM layer and some Dropout regularisation
model.add(LSTM(units = 4, return_sequences = True, input_shape = (1, look_back)))
model.add(Dropout(0.2))
# Adding a second LSTM layer and some Dropout regularisation
model.add(LSTM(units = 30, return_sequences = True))
model.add(Dropout(0.2))
# Adding a third LSTM layer and some Dropout regularisation
model.add(LSTM(units = 30, return_sequences = True))
model.add(Dropout(0.2))
# Adding a fourth LSTM layer and some Dropout regularisation
model.add(LSTM(units = 4))
model.add(Dropout(0.2))

model.add(Dense(1))
model.compile(loss='mean_squared_error', optimizer='adam')
model.fit(trainX, trainY, epochs=epochNumber, batch_size=1, verbose=2)

model.save(inputfile + '_model')
