import { connect } from 'react-redux';
import { changeQuantity, cancelBasket, checkoutBasket } from '../actions';
import Basket from '../components/Basket';

const mapStateToProps = (state) => {
    return {
        basket: state.basket
    };
}

const mapDispatchToProps = (dispatch) => {
    return {
        changeQuantity: (productId, quantity) => {
            dispatch(changeQuantity(productId, quantity));
        },
        checkoutBasket: (basketId, address) => {
            dispatch(checkoutBasket(basketId, address));
        },
        cancelBasket: (basketId) => {
            dispatch(cancelBasket(basketId));
        }

    };
}

const BasketContainer = connect(mapStateToProps, mapDispatchToProps)(Basket);

export default BasketContainer;