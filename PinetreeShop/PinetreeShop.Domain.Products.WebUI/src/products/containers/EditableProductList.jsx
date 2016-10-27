import { connect } from 'react-redux';
import { addProduct, setProductQuantity, filterProducts } from '../actions';
import ProductList from '../components/ProductList';

const getFilteredProducts = (products, filter) => {
    return products.filter(p => p.name.indexOf(filter) >= 0);    
}

const mapStateToProps = (state) => {
    return {
        products: getFilteredProducts(state.products, state.filter)
    };
}

const mapDispatchToProps = (dispatch) => {
    return {
        addProduct: (name, price) => {
            dispatch(addProduct(name, price));
        },
        setQuantity: (id, diff) => {
            dispatch(setProductQuantity(id, diff));
        },
        filterProducts: (text) => {
            dispatch(filterProducts(text));
        }
    };
}

const EditableProductList = connect(mapStateToProps, mapDispatchToProps)(ProductList);

export default EditableProductList;