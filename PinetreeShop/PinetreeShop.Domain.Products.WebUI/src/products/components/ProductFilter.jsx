import React from 'react';
import { InputGroup, FormControl } from 'react-bootstrap'


class ProductFilter extends React.Component {
    constructor(props){
        super(props);
        this.handleChange = this.handleChange.bind(this);
        this.state = {
            value: ''
        };
    }

    handleChange(e) {
        this.setState({ value: e.target.value });
        this.props.filterProducts(e.target.value);
    }

    render() {
        return (
            <InputGroup>
                <InputGroup.Addon>Filter:</InputGroup.Addon>
                <FormControl 
                    value={this.state.value}
                    type='text' 
                    onChange={this.handleChange}/>
            </InputGroup>
        );
    }
}

export default ProductFilter;