
rastr = function(args){
  return(sum(args^2-10*cos(2*pi*args)))
}

shvel = function(args){
  return(sum(-args*sin(sqrt(abs(args)))))
}



x=seq(-5,5,length.out = 150)

plot(x, sapply(x, rastr), type ='l',col = 'green', lwd = 3, 
     main = "Rastrigin function 1D", ylab = 'f(x)',
     sub = 'min = -10, argmin = 0')


x=seq(-150,150,length.out = 150)
plot(x, sapply(x, shvel), type ='l',col = 'red', lwd = 3, main = "Shvel function 1D", ylab = 'f(x)')


plot(x, sapply(x, function(t) rastr(t)*cos(t)), type ='l')



get.mat = function(x,y,fun){
  z = matrix(nrow = length(x), ncol = length(y))
  for(i in seq(x))
    for(j in seq(y)){
      z[i,j]=fun(c(i,j))
    }
  
  return(z)
}

x = x

y = x

ur = get.mat(x,y, shvel)



library(rgl)
library(plot3D)
library(viridis)
library(gridExtra)
library(fields)


levels = 30
par(mfrow = c(2, 2))
layout(matrix(c(1,2, 3,4), 2, byrow = T), heights = c(1.5, 1))

noize = matrix(runif(length(x)*length(y),-1,1),nrow = nrow(ur))

persp3D(z = ur, x = x, y = y, scale = T,
             contour = list(nlevels = levels, col = "red"),
             #zlim = c(),
             expand = 0.3,
             image = list(col = grey(seq(0, 1, length.out = 100))), main = "Shvel function 2D")
persp3D(z = ur+noize, x = x, y = y, scale = T,
        contour = list(nlevels = levels, col = "red"),
        #zlim = c(),
        expand = 0.3,
        image = list(col = grey(seq(0, 1, length.out = 100))), main = "Shvel function 2D + noise")

image(x, y, ur, col = heat.colors(levels), main = "Shvel function")
image(x, y, ur+noize, col = heat.colors(levels), main = "Shvel function + noise")

























