import fetch from 'isomorphic-fetch';
import config from '../config';
import { dispatchBasketUpdate } from '../baskets/actions';

const createRequest = (method, body) => ({
    method,
    body,
    headers: {
        'Content-Type': 'application/json'
    }
})

const createPostRequest = (body) => createRequest('POST', JSON.stringify(body));
const createGetRequest = () => createRequest('GET');

export const actionTypes = {
    UPDATE_PRODUCTS: 'UPDATE_PRODUCTS',
    SET_PRODUCT_QUANTITY: 'SET_PRODUCT_QUANTITY',
    ADD_TO_BASKET: 'ADD_TO_BASKET'
}

export const getProducts = () => {
    return dispatch => {
        const url = config.PRODUCTS_API_URL + '/list';

        fetch(url, createGetRequest(url))
            .then(response => response.json())
            .then(products => {
                dispatch({
                    type: actionTypes.UPDATE_PRODUCTS,
                    products
                });
            });
    }
}

export const addToBasket = (productId, quantity) => {
    return (dispatch, getState) => {        
        const state = getState();
        const product = state.products.find(p => p.id === productId);

        const url = config.BASKET_API_URL + '/addItem';
        const postModel = { 
            basketId: state.basket.id,
            productId,
            name: product.name,
            price: product.price,
            quantity 
        };

        fetch(url, createPostRequest(postModel))
            .then(response => response.json())        
            .then(basket => dispatchBasketUpdate(dispatch, basket));
    };
}